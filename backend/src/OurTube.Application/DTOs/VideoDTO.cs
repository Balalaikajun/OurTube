using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs
{
    public class VideoDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int LikesCount { get; set; }
        public int DislikeCount { get; set; } 
        public int CommentsCount { get; set; }
        public int ViewsCount { get; set; }
        public DateTime Created { get; set; }
        public string ApplicationUserId { get; set; }
        public VideoPreviewDTO Preview { get; set; }
        public List<VideoPlaylistDTO> Files { get; set; }
        public ApplicationUserDTO User { get; set; }

    }
}
