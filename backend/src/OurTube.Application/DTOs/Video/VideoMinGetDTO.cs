using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.VideoPreview;

namespace OurTube.Application.DTOs.Video
{
    public class VideoMinGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ViewsCount { get; set; }
        public bool? Vote { get; set; }
        public long? EndTime { get; set; }
        public DateTime Created { get; set; }
        public VideoPreviewDTO Preview { get; set; }
        public ApplicationUserDTO User { get; set; }
    }
}
