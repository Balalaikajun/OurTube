using AutoMapper;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VideoService _videoService;
        private readonly IMapper _mapper;

        public PlaylistService(IUnitOfWork unitOfWork, VideoService videoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task CreateAsync(PlaylistPostDto playlistDto, string userId)
        {
            var playlist = new Playlist()
            {
                Title = playlistDto.Title,
                Description = playlistDto.Description,
                ApplicationUserId = userId
            };

            _unitOfWork.Playlists.Add(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(PlaylistPatchDto patchDto, int playlistId, string userId)
        {
            var playlist =await _unitOfWork.Playlists.GetAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.IsSystem || playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (patchDto.Title != null)
                playlist.Title = patchDto.Title;

            if (patchDto.Description != null)
                playlist.Description = patchDto.Description;

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int playlistId, string userId)
        {
            var playlist =await _unitOfWork.Playlists.GetAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.IsSystem || playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");
            
            
            _unitOfWork.Playlists.Remove(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddVideoAsync(int playlistId, int videoId, string userId)
        {
            var playlist =await _unitOfWork.Playlists.GetAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if ( playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var element = await _unitOfWork.PlaylistElements.GetAsync(playlistId, videoId);
            
            if (element != null)
                return;
            
            element = new PlaylistElement( videoId, playlistId, userId);

            _unitOfWork.PlaylistElements.Add(element);

            playlist.Count++;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveVideoAsync(int playlistId, int videoId, string userId)
        {
            var playlist =await _unitOfWork.Playlists.GetAsync(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if ( playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var playlistElement =await _unitOfWork.PlaylistElements.GetAsync(playlistId, videoId);

            if (playlistElement == null)
                return;

            playlistElement.DeleteEvent(userId);
            _unitOfWork.PlaylistElements.Remove(playlistElement);
            playlist.Count--;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PlaylistGetDto> GetWithLimitAsync(int playlistId, string userId, int limit, int after)
        {
            var playlist = await _unitOfWork.Playlists.GetPlaylistWithElementsAsync(playlistId, limit, after);

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


            return (await _unitOfWork.Playlists.FindAsync(p => p.ApplicationUserId == userId))
                .Select(p => _mapper.Map<PlaylistMinGetDto>(p));
        }

        public async Task<PlaylistMinGetDto> GetLikedPlaylistAsync(string userId)
        {
            var playlist =(await _unitOfWork.Playlists
                    .FindAsync(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId))
                .FirstOrDefault();

            if (playlist == null)
            {
                await CreateAsync(new DTOs.Playlist.PlaylistPostDto { Title = "Понравившееся" }, userId);
                playlist =(await _unitOfWork.Playlists
                        .FindAsync(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId))
                    .First();
            }
            
            return _mapper.Map<PlaylistMinGetDto>(playlist);
        }

    }
}
