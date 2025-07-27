using MediatR;
using OurTube.Application.Interfaces;
using OurTube.Domain.Events.VideoVote;

namespace OurTube.Application.Handlers;

public class PlaylistLikeHandler :
    INotificationHandler<VideoVoteCreateEvent>,
    INotificationHandler<VideoVoteUpdateEvent>,
    INotificationHandler<VideoVoteDeleteEvent>
{
    private readonly IPlaylistCrudService _playlistCrudService;
    private readonly IPlaylistQueryService _playlistQueryService;


    public PlaylistLikeHandler(IPlaylistCrudService playlistCrudService, IPlaylistQueryService playlistQueryService)
    {
        _playlistCrudService = playlistCrudService;
        _playlistQueryService = playlistQueryService;
    }

    public async Task Handle(VideoVoteCreateEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.Value)
            return;

        var playlist = await _playlistQueryService.GetLikedPlaylistAsync(notification.UserId);

        await _playlistCrudService.AddVideoAsync(playlist.Id, notification.VideoId);
    }

    public async Task Handle(VideoVoteDeleteEvent notification, CancellationToken cancellationToken)
    {
        if (!notification.Value)
            return;

        var playlist = await _playlistQueryService.GetLikedPlaylistAsync(notification.UserId);

        await _playlistCrudService.RemoveVideoAsync(playlist.Id, notification.VideoId, true);
    }

    public async Task Handle(VideoVoteUpdateEvent notification, CancellationToken cancellationToken)
    {
        var playlist = await _playlistQueryService.GetLikedPlaylistAsync(notification.UserId);

        if (notification.NewValue)
            await _playlistCrudService.AddVideoAsync(playlist.Id, notification.VideoId);
        else
            await _playlistCrudService.RemoveVideoAsync(playlist.Id, notification.VideoId, true);
    }
}