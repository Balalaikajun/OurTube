using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.PlaylistElement;

public class PlaylistElementDeleteEvent(int PlaylistId, int VideoId, string UserId, DateTime Created) : IDomainEvent
{
    public int PlaylistId { get; } = PlaylistId;
    public int VideoId { get; } = VideoId;
    public string UserId { get; } = UserId;
    public DateTime Created { get; } = Created;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}