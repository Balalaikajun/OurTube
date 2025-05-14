using MediatR;
using OurTube.Application.Services;
using OurTube.Domain.Events.VideoVote;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Handlers;

public class PlaylistLikeHandler: 
    INotificationHandler<VideoVoteCreateEvent>,
    INotificationHandler<VideoVoteUpdateEvent>,
    INotificationHandler<VideoVoteDeleteEvent>
{
    private readonly PlaylistService _playlistService;

    public PlaylistLikeHandler(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }
    
    public async Task Handle(VideoVoteCreateEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.Value)
            return;
        
        var playlist = await _playlistService.GetLikedPlaylistAsync(notification.UserId);
        
        await _playlistService.AddVideoAsync(playlist.Id, notification.VideoId, notification.UserId);
    }
   
    public async Task Handle(VideoVoteUpdateEvent notification, CancellationToken cancellationToken)
    {
        
        var playlist = await _playlistService.GetLikedPlaylistAsync(notification.UserId);
        
        if (notification.NewValue)
        {
            await _playlistService.AddVideoAsync(playlist.Id, notification.VideoId, notification.UserId);
        }
        else
        {
            await _playlistService.RemoveVideoAsync(playlist.Id, notification.VideoId, notification.UserId, true);
        }
    } 
    
    public async Task Handle(VideoVoteDeleteEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.Value)
            return;
        
        var playlist = await _playlistService.GetLikedPlaylistAsync(notification.UserId);
        
        await _playlistService.RemoveVideoAsync(playlist.Id, notification.VideoId, notification.UserId);
    }
}