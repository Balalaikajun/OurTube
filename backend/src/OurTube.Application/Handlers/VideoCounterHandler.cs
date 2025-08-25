using MediatR;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Events.VideoVote;

namespace OurTube.Application.Handlers;

public class VideoCounterHandler :
    INotificationHandler<VideoVoteCreateEvent>,
    INotificationHandler<VideoVoteUpdateEvent>,
    INotificationHandler<VideoVoteDeleteEvent>
{
    private readonly IApplicationDbContext _dbContext;

    public VideoCounterHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(VideoVoteCreateEvent notification, CancellationToken cancellationToken)
    {
        var video = await _dbContext.Videos
            .GetByIdAsync(notification.VideoId, true);

        if (notification.Value)
            video.UpdateLikesCount(1);
        else
            video.UpdateDislikesCount(1);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(VideoVoteDeleteEvent notification, CancellationToken cancellationToken)
    {
        var video = await _dbContext.Videos
            .GetByIdAsync(notification.VideoId, true);

        if (notification.Value)
            video.UpdateLikesCount(-1);
        else
            video.UpdateDislikesCount(-1);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(VideoVoteUpdateEvent notification, CancellationToken cancellationToken)
    {
        if (notification.OldValue == notification.NewValue)
            return;

        var video = await _dbContext.Videos
            .GetByIdAsync(notification.VideoId, true);

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

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}