using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteDeleteEvent(Guid VideoId, Guid UserId, bool Value) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}