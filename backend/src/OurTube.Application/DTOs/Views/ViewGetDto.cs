using OurTube.Application.DTOs.Video;

namespace OurTube.Application.DTOs.Views;

public class ViewGetDto
{
    public VideoMinGetDto Video { get; set; }
    public DateTime DateTime { get; set; }
}