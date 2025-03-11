using OurTube.Application.DTOs.Video;

namespace OurTube.Application.DTOs.PlaylistElement
{
    public class PlaylistElementGetDTO
    {

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public VideoMinGetDTO Video { get; set; }
    }
}
