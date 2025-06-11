using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Domain.Entities;

public class PlaylistElement : BaseEntity
{
    public PlaylistElement()
    {
    }

    public PlaylistElement(int playlistId, int videoId, string userId)
    {
        PlaylistId = playlistId;
        VideoId = videoId;

        AddDomainEvent(new PlaylistElementCreateEvent(PlaylistId, VideoId, userId, AddedAt));
    }

    public int PlaylistId { get; }
    public int VideoId { get; }
    public DateTime AddedAt { get; } = DateTime.UtcNow;

    //Navigation
    public Playlist Playlist { get; }
    public Video Video { get; }

    public void DeleteEvent(string userId)
    {
        AddDomainEvent(new PlaylistElementDeleteEvent(PlaylistId, VideoId, userId, AddedAt));
    }
}