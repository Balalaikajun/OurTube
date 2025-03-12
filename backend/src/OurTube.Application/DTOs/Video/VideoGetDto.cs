using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.VideoPlaylist;
using OurTube.Application.DTOs.VideoPreview;

namespace OurTube.Application.DTOs.Video
{
    public class VideoGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikesCount { get; set; }
        public int DislikeCount { get; set; }
        public int CommentsCount { get; set; }
        public int ViewsCount { get; set; }
        public bool? Vote { get; set; }
        public long? EndTime { get; set; }
        public DateTime Created { get; set; }
        public VideoPreviewDTO Preview { get; set; }
        public List<VideoPlaylistDTO> Files { get; set; }
        public ApplicationUserDTO User { get; set; }

    }
}
