using OurTube.Application.DTOs.PlaylistElement;

namespace OurTube.Application.DTOs.Playlist
{
    public class PlaylistGetDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<PlaylistElementGetDTO> PlaylistElements { get; set; }

    }
}
