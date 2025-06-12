using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.PlaylistElement;

public record PlaylistElementDeleteEvent(
    int PlaylistId,
    string PlaylistTitle,
    int VideoId,
    bool IsSystem,
    string UserId,
    DateTime Created
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}