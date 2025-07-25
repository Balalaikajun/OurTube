using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Domain.Entities;
using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Application.Services;

public class PlaylistService : IPlaylistCrudService, IPlaylistQueryService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public PlaylistService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<PlaylistMinGetDto> CreateAsync(PlaylistPostDto playlistDto, Guid userId)
    {
        var playlist = new Playlist
        {
            Title = playlistDto.Title,
            Description = playlistDto.Description,
            ApplicationUserId = userId
        };

        _dbContext.Playlists.Add(playlist);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<PlaylistMinGetDto>(playlist);
    }

    public async Task UpdateAsync(PlaylistPatchDto patchDto, Guid playlistId, Guid userId)
    {
        var playlist = await _dbContext.Playlists.FindAsync(playlistId);

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

    public async Task DeleteAsync(Guid playlistId, Guid userId)
    {
        var playlist = await _dbContext.Playlists.FindAsync(playlistId);

        if (playlist == null)
            throw new KeyNotFoundException("Плейлист не найден");

        if (playlist.IsSystem || playlist.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");


        playlist.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddVideoAsync(Guid playlistId, Guid videoId, Guid userId)
    {
        var playlist = await _dbContext.Playlists.FindAsync(playlistId);

        if (playlist == null)
            throw new KeyNotFoundException("Плейлист не найден");

        if (playlist.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

        var element = await _dbContext.PlaylistElements.FindAsync(playlistId, videoId);

        if (element != null)
            return;

        element = new PlaylistElement(playlist, videoId, userId);

        _dbContext.PlaylistElements.Add(element);

        playlist.Count++;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        Guid userId,
        bool suppressDomainEvent = false)
    {
        var playlist = await _dbContext.Playlists.FindAsync(playlistId);

        if (playlist == null)
            throw new KeyNotFoundException("Плейлист не найден");

        if (playlist.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

        var playlistElement = await _dbContext.PlaylistElements.FindAsync(playlistId, videoId);

        if (playlistElement == null)
            return;

        if (!suppressDomainEvent) 
            playlistElement.AddDomainEvent(
                new PlaylistElementDeleteEvent(
                    playlist.Id,
                    playlist.Title,
                    playlist.IsSystem,
                    playlist.ApplicationUserId, 
                    playlistElement.VideoId));

        playlistElement.Delete();
        playlist.Count--;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedDto<PlaylistElementGetDto>> GetElements(Guid playlistId, Guid userId, int limit, int after)
    {
        var playlist = await _dbContext.Playlists
            .FirstOrDefaultAsync(x => x.Id == playlistId);

        if (playlist == null)
            throw new KeyNotFoundException("Плейлист не найден");

        if (playlist.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");
        
        var playlistElements = await _dbContext.PlaylistElements
            .Where(x => x.PlaylistId == playlistId)
            .OrderBy(x => x.CreatedDate)
            .Skip(after)
            .Take(limit + 1)
            .ToListAsync();

        var hasMore = playlistElements.Count > limit;
        playlistElements = playlistElements.Take(limit).ToList();

        var videoIds = playlistElements.Take(limit).Select(x => x.VideoId).ToList();

        var videos = await _dbContext.Videos
            .Where(x => videoIds.Contains(x.Id))
            .ProjectToMinDto(_mapper, userId)
            .ToDictionaryAsync(x => x.Id, x => x);

        var elementsDto = playlistElements.Select(x => new PlaylistElementGetDto
        {
            Video = videos[x.VideoId],
            AddedAt = x.CreatedDate
        });

        return new PagedDto<PlaylistElementGetDto>()
        {
            Elements = elementsDto,
            HasMore = hasMore,
            NextAfter = after+limit
        };
    }

    public async Task<IEnumerable<PlaylistMinGetDto>> GetUserPlaylistsAsync(Guid userId)
    {
        return await _dbContext.Playlists
            .Where(p => p.ApplicationUserId == userId)
            .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlaylistForVideoGetDto>> GetUserPlaylistsForVideoAsync(Guid userId, Guid videoId)
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

    public async Task<PlaylistMinGetDto> GetLikedPlaylistAsync(Guid userId)
    {
        var playlist = await _dbContext.Playlists
            .Where(p => p.ApplicationUserId == userId && p.Title == "Понравившееся" && p.IsSystem == true)
            .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (playlist == null)
        {
            await CreateAsync(new PlaylistPostDto { Title = "Понравившееся" }, userId);
            playlist = await _dbContext.Playlists
                .Where(p => p.ApplicationUserId == userId && p.Title == "Понравившееся" && p.IsSystem == true)
                .ProjectTo<PlaylistMinGetDto>(_mapper.ConfigurationProvider)
                .FirstAsync();
        }

        return playlist;
    }

    public async Task<PlaylistMinGetDto> GetMinById(Guid id, Guid userId)
    {
        var playlist = await _dbContext.Playlists
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (playlist == null)
            throw new KeyNotFoundException("Playlist not found");
        
        if(playlist.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Unauthorized access");
        
        return _mapper.Map<PlaylistMinGetDto>(playlist);
    }
}