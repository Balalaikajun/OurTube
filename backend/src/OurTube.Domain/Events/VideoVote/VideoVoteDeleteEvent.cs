using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.VideoVote;

public record VideoVoteDeleteEvent(int VideoId, string UserId, bool Value) : IDomainEvent
{
    public int VideoId { get; } = VideoId;
    public string UserId { get; } = UserId;
    public bool Value { get; } = Value;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}