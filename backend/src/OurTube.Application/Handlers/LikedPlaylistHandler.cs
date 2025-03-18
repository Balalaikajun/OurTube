using MediatR;
using OurTube.Application.Services;
using OurTube.Domain.Entities;
using OurTube.Domain.Events.PlaylistElement;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Handlers;

public class LikedPlaylistHandler:
    INotificationHandler<PlaylistElementCreateEvent>,
    INotificationHandler<PlaylistElementDeleteEvent>
{
    private VideoVoteService _videoVoteService;
    private IUnitOfWork _unitOfWork;

    
    public LikedPlaylistHandler(VideoVoteService videoVoteService, IUnitOfWork unitOfWork)
    {
        _videoVoteService = videoVoteService;
        _unitOfWork = unitOfWork;
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