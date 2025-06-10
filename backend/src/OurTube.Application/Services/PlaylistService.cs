using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly VideoService _videoService;
        private readonly IMapper _mapper;

        public PlaylistService(IApplicationDbContext dbContext, VideoService videoService, IMapper mapper)
        {
            _dbContext = dbContext;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task<PlaylistMinGetDto> CreateAsync(PlaylistPostDto playlistDto, string userId)
        {
            var playlist = new Playlist()
            {
                Title = playlistDto.Title,
                Description = playlistDto.Description,
                ApplicationUserId = userId
            };

            _dbContext.Playlists.Add(playlist);

            await _dbContext.SaveChangesAsync();
            
            return _mapper.Map<PlaylistMinGetDto>(playlist);
        }

        public async Task UpdateAsync(PlaylistPatchDto patchDto, int playlistId, string userId)
        {
            var playlist =await _dbContext.Playlists.FindAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.IsSystem || playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (patchDto.Title != null)
                playlist.Title = patchDto.Title;

            if (patchDto.Description != null)
                playlist.Description = patchDto.Description;

            await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int playlistId, string userId)
        {
            var playlist =await _dbContext.Playlists.FindAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.IsSystem || playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");
            
            
            _dbContext.Playlists.Remove(playlist);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddVideoAsync(int playlistId, int videoId, string userId)
        {
            var playlist =await _dbContext.Playlists.FindAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if ( playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var element = await _dbContext.PlaylistElements.FindAsync(playlistId, videoId);
            
            if (element != null)
                return;
            
            element = new PlaylistElement(playlistId, videoId, userId);

            _dbContext.PlaylistElements.Add(element);

            playlist.Count++;

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveVideoAsync(
            int playlistId, 
            int videoId,
            string userId,
            bool suppressDomainEvent = false)
        {
            var playlist =await _dbContext.Playlists.FindAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if ( playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var playlistElement =await _dbContext.PlaylistElements.FindAsync(playlistId, videoId);

            if (playlistElement == null)
                return;

            if (!suppressDomainEvent)
            {
                playlistElement.DeleteEvent(userId);
            }
            
            _dbContext.PlaylistElements.Remove(playlistElement);
            playlist.Count--;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PlaylistGetDto> GetWithLimitAsync(int playlistId, string userId, int limit, int after)
        {
            var playlist = await _dbContext.Playlists
                .Include(p => p.PlaylistElements
                    .OrderBy(pe => pe.AddedAt)
                    .Skip(after)
                    .Take(limit))
                .FirstOrDefaultAsync(x => x.Id == playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var playlistGetDto = _mapper.Map<PlaylistGetDto>(playlist);

            playlistGetDto.PlaylistElements = [];
            foreach (var pe in playlist.PlaylistElements)
            {
                playlistGetDto.PlaylistElements.Add(new PlaylistElementGetDto()
                {
                    AddedAt = pe.AddedAt,
                    Video = await _videoService.GetMinVideoByIdAsync(pe.VideoId, userId)

                });
            }

            return playlistGetDto;
        }

        public async Task<IEnumerable<PlaylistMinGetDto>> GetUserPlaylistsAsync(string userId)
        {
            return await _dbContext.Playlists
                    .Where(p => p.ApplicationUserId == userId)
                    .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
        }
        
        public async Task<IEnumerable<PlaylistForVideoGetDto>> GetUserPlaylistsForVideoAsync(string userId, int videoId)
        {
            return await _dbContext.Playlists
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => new PlaylistForVideoGetDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Count = p.Count,
                    HasVideo = p.PlaylistElements.Any(pe => pe.VideoId == videoId)
                })
                .ToListAsync();
        }

        public async Task<PlaylistMinGetDto> GetLikedPlaylistAsync(string userId)
        {
            var playlist = await _dbContext.Playlists
                .Where(p => p.ApplicationUserId == userId && p.Title == "Понравившееся")
                .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (playlist == null)
            {
                await CreateAsync(new PlaylistPostDto { Title = "Понравившееся" }, userId);
                playlist = await _dbContext.Playlists
                    .Where(p => p.ApplicationUserId == userId && p.Title == "Понравившееся")
                    .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
                    .FirstAsync();
            }

            return playlist;
        }

    }
}
