using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Events.PlaylistElement;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(PlaylistId), nameof(VideoId))]
    public class PlaylistElement:BaseEntity
    {
        public int PlaylistId { get; private set; }
        public int VideoId { get; private set; }

        public DateTime AddedAt { get; private set; } = DateTime.UtcNow;

        //Navigation
        public Playlist Playlist { get; private set; }
        public Video Video { get; private set; }

        public PlaylistElement()
        {
            
        }
        public PlaylistElement(int playlistId, int videoId, string userId)
        {
            PlaylistId = playlistId;
            VideoId = videoId;
            
            AddDomainEvent( new PlaylistElementCreateEvent(PlaylistId, VideoId, userId, AddedAt) );
        }

        public void DeleteEvent(string userId)
        {
            AddDomainEvent(new PlaylistElementDeleteEvent(PlaylistId, VideoId,userId, AddedAt));
        }
    }
}
