using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Events.PlaylistElement;

public record PlaylistElementDeleteEvent(
    Guid PlaylistId,
    string PlaylistTitle,
    bool IsSystem,
    Guid UserId,
    Guid VideoId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}