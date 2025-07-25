using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteCreateEvent(
    Guid VideoId,
    Guid UserId,
    bool Value) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}