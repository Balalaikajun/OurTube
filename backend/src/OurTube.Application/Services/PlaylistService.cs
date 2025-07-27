using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Playlist;
using OurTube.Application.Requests.Playlist;
using OurTube.Domain.Entities;
using OurTube.Domain.Events.PlaylistElement;
using Playlist = OurTube.Domain.Entities.Playlist;

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

    public async Task<Replies.Playlist.Playlist> CreateAsync(Guid userId,PostPlaylistRequest request)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);
        
        var playlist = new Playlist
        {
            Title = request.Title,
            ApplicationUserId = userId
        };

        _dbContext.Playlists.Add(playlist);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Replies.Playlist.Playlist>(playlist);
    }

    public async Task UpdateAsync(Guid playlistId, UpdatePlaylistRequest request) 
    {
        var playlist = await _dbContext.Playlists
            .GetByIdAsync(playlistId, true);

        if (!string.IsNullOrWhiteSpace(request.Title))
            playlist.Title = request.Title;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var playlist = await _dbContext.Playlists
            .GetByIdAsync(id,true);

        playlist.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddVideoAsync(Guid playlistId, Guid videoId)
    {
        var playlist = await _dbContext.Playlists
            .GetByIdAsync(playlistId, true);
        
        await _dbContext.PlaylistElements
            .EnsureNotExistAsync(x => x.PlaylistId == playlistId && x.VideoId == videoId);

        var element = new PlaylistElement(playlist, videoId, playlist.ApplicationUserId);

        _dbContext.PlaylistElements.Add(element);

        playlist.Count++;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        bool suppressDomainEvent = false)
    {
        var playlist = await _dbContext.Playlists
            .GetByIdAsync(playlistId, true);

        var playlistElement = await _dbContext.PlaylistElements
            .GetAsync(pe => pe.PlaylistId == playlistId && pe.VideoId == videoId, true);

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

    public async Task<ListReply<Replies.PlaylistElement.PlaylistElement>> GetElements(Guid playlistId, Guid userId, int limit, int after)
    {
        await _dbContext.Playlists
            .EnsureExistAsync(playlistId);
        
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

        var elementsDto = playlistElements.Select(x => new Replies.PlaylistElement.PlaylistElement()
        {
            Video = videos[x.VideoId],
            CreatedDate = x.CreatedDate
        });

        return new ListReply<Replies.PlaylistElement.PlaylistElement>()
        {
            Elements = elementsDto,
            HasMore = hasMore,
            NextAfter = after+limit
        };
    }

    public async Task<IEnumerable<Replies.Playlist.Playlist>> GetUserPlaylistsAsync(Guid userId)
    {
        return await _dbContext.Playlists
            .Where(p => p.ApplicationUserId == userId)
            .ProjectTo<Replies.Playlist.Playlist>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<PlaylistForVideo>> GetUserPlaylistsForVideoAsync(Guid userId, Guid videoId)
    {
        return await _dbContext.Playlists
            .Where(p => p.ApplicationUserId == userId)
            .Select(p => new PlaylistForVideo()
            {
                Id = p.Id,
                Title = p.Title,
                Count = p.Count,
                HasVideo = p.PlaylistElements.Any(pe => pe.VideoId == videoId)
            })
            .ToListAsync();
    }

    public async Task<Replies.Playlist.Playlist> GetLikedPlaylistAsync(Guid userId)
    {
        var playlist = await _dbContext.Playlists
            .Where(p => p.ApplicationUserId == userId && p.IsSystem == true && p.Title == "Понравившееся" )
            .ProjectTo<Replies.Playlist.Playlist>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return playlist;
    }

    public async Task<Replies.Playlist.Playlist> GetMinById(Guid id)
    {
        var playlist = await _dbContext.Playlists
            .GetByIdAsync(id);
        
        return _mapper.Map<Replies.Playlist.Playlist>(playlist);
    }
}