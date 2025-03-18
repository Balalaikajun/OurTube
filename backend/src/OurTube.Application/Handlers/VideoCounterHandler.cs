using MediatR;
using OurTube.Domain.Events.VideoVote;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Persistence;

namespace OurTube.Application.Handlers;

public class VideoCounterHandler:
    INotificationHandler<VideoVoteCreateEvent>,
    INotificationHandler<VideoVoteUpdateEvent>,
    INotificationHandler<VideoVoteDeleteEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public VideoCounterHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VideoVoteCreateEvent notification, CancellationToken cancellationToken)
    {
        var video =await _unitOfWork.Videos.GetAsync(notification.VideoId);
        
        if(video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (notification.Value)
        {
            video.UpdateLikesCount(1);
        }
        else
        {
            video.UpdateDislikesCount(1);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task Handle(VideoVoteUpdateEvent notification, CancellationToken cancellationToken)
    {
        if(notification.OldValue == notification.NewValue)
            return;
        
        var video =await _unitOfWork.Videos.GetAsync(notification.VideoId);
        
        if(video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (notification.NewValue)
        {
            video.UpdateLikesCount(1);
            video.UpdateDislikesCount(-1);
        }
        else
        {
            video.UpdateDislikesCount(1);
            video.UpdateLikesCount(-1);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);  
    }
    
    public async Task Handle(VideoVoteDeleteEvent notification, CancellationToken cancellationToken)
    {
        var video =await _unitOfWork.Videos.GetAsync(notification.VideoId);
        
        if(video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (notification.Value)
        {
            video.UpdateLikesCount(-1);
        }
        else
        {
            video.UpdateDislikesCount(-1);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);  
    }
}