using Microsoft.EntityFrameworkCore;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(PlaylistId), nameof(VideoId))]
    public class PlaylistElement
    {
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        //Navigation
        public Playlist Playlist { get; set; }
        public Video Video { get; set; }
    }
}
