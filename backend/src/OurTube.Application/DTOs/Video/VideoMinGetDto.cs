using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.VideoPreview;

namespace OurTube.Application.DTOs.Video;

public class VideoMinGetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int ViewsCount { get; set; }
    public bool? Vote { get; set; }

    public TimeSpan Duration { get; set; }
    public TimeSpan? EndTime { get; set; }
    public DateTime Created { get; set; }
    public VideoPreviewDto Preview { get; set; }
    public ApplicationUserDto User { get; set; }
}