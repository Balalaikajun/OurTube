using MediatR;
using OurTube.Application.Services;
using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Application.Handlers;

public class LikedPlaylistHandler :
    INotificationHandler<PlaylistElementCreateEvent>,
    INotificationHandler<PlaylistElementDeleteEvent>
{
    private readonly VideoVoteService _videoVoteService;


    public LikedPlaylistHandler(VideoVoteService videoVoteService)
    {
        _videoVoteService = videoVoteService;
    }

    public async Task Handle(PlaylistElementCreateEvent notification, CancellationToken cancellationToken)
    {
        await _videoVoteService.SetAsync(notification.VideoId, notification.UserId, true);
    }

    public async Task Handle(PlaylistElementDeleteEvent notification, CancellationToken cancellationToken)
    {
        await _videoVoteService.DeleteAsync(notification.VideoId, notification.UserId);
    }
}