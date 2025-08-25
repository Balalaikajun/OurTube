using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteUpdateEvent(Guid VideoId, Guid UserId, bool OldValue, bool NewValue)
    : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}