using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteUpdateEvent(int VideoId, string UserId, bool OldValue, bool NewValue, DateTime Created)
    : IDomainEvent
{
    public int VideoId { get; } = VideoId;
    public string UserId { get; } = UserId;
    public bool OldValue { get; } = OldValue;
    public bool NewValue { get; } = NewValue;
    public DateTime Created { get; } = Created;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}