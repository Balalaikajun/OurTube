using OurTube.Application.DTOs.Video;

namespace OurTube.Application.DTOs.PlaylistElement;

public class PlaylistElementGetDto
{
    public DateTime AddedAt { get; set; } 
    public VideoMinGetDto Video { get; set; }
}