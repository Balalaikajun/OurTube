using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.PlaylistElement;

public record PlaylistElementCreateEvent(
    Guid PlaylistId,
    string PlaylistTitle,
    bool IsSystem,
    Guid UserId,
    Guid VideoId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}