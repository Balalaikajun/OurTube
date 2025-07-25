using MediatR;
using OurTube.Application.Interfaces;
using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Application.Handlers;

public class LikedPlaylistHandler :
    INotificationHandler<PlaylistElementCreateEvent>,
    INotificationHandler<PlaylistElementDeleteEvent>
{
    private readonly IVideoVoteService _videoVoteService;

    public LikedPlaylistHandler(IVideoVoteService videoVoteService)
    {
        _videoVoteService = videoVoteService;
    }

    public async Task Handle(PlaylistElementCreateEvent notification, CancellationToken cancellationToken)
    {
        if (notification.IsSystem && notification.PlaylistTitle == "Понравившееся")
            await _videoVoteService.SetAsync(notification.VideoId, notification.UserId, true);
    }

    public async Task Handle(PlaylistElementDeleteEvent notification, CancellationToken cancellationToken)
    {
        if (notification.IsSystem && notification.PlaylistTitle == "Понравившееся")
            await _videoVoteService.DeleteAsync(notification.VideoId, notification.UserId);
    }
}