namespace OurTube.Application.DTOs.Video;

public class PagedVideoDto
{
    public IEnumerable<VideoMinGetDto> Videos { get; set; }
    public int NextAfter { get; set; }
    public bool HasMore { get; set; }
}