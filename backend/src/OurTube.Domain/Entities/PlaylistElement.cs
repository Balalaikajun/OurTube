using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Domain.Entities;

public class PlaylistElement : BaseEntity
{
    public PlaylistElement()
    {
    }

    public PlaylistElement(int playlistId, string playlistTitle, bool isSystem, int videoId, string userId)
    {
        PlaylistId = playlistId;
        VideoId = videoId;

        AddDomainEvent(new PlaylistElementCreateEvent(
            playlistId,
            playlistTitle,
            videoId,
            isSystem,
            userId,
            AddedAt));
    }


    public int PlaylistId { get; }
    public int VideoId { get; }
    public DateTime AddedAt { get; } = DateTime.UtcNow;

    //Navigation
    public Playlist Playlist { get; }
    public Video Video { get; }

    public void InitializeCreateEvent(string userId)
    {
        if (Playlist == null)
            throw new InvalidOperationException("Playlist must be loaded before initializing the event.");

        AddDomainEvent(new PlaylistElementCreateEvent(
            PlaylistId,
            Playlist.Title,
            VideoId,
            Playlist.IsSystem,
            userId,
            AddedAt));
    }
}