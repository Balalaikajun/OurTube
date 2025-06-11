using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteCreateEvent(int VideoId, string UserId, bool Value, DateTime Created) : IDomainEvent
{
    public int VideoId { get; } = VideoId;
    public string UserId { get; } = UserId;
    public bool Value { get; } = Value;
    public DateTime Created { get; } = Created;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}