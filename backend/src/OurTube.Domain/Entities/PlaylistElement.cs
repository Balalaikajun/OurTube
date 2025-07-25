using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Domain.Entities;

public class PlaylistElement : Base
{
    public PlaylistElement()
    {
    }

    public PlaylistElement(Playlist playlist, Guid videoId, Guid userId)
    {
        PlaylistId = playlist.Id;
        VideoId = videoId;

        AddDomainEvent(new PlaylistElementCreateEvent(
            PlaylistId,
            playlist.Title,
            playlist.IsSystem,
            userId,
            videoId));
    }

    public Guid PlaylistId { get; }
    public Guid VideoId { get; }

    //Navigation
    public Playlist Playlist { get; }
    public Video Video { get; }
    
    public override void Delete()
    {
        base.Delete();
    }
}