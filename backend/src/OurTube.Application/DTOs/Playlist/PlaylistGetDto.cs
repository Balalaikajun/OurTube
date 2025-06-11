using OurTube.Application.DTOs.PlaylistElement;

namespace OurTube.Application.DTOs.Playlist;

public class PlaylistGetDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Count { get; set; }
    public List<PlaylistElementGetDto> PlaylistElements { get; set; }
}