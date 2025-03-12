using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Video
{
    public class VideoUploadDTO
    {
        [Required]
        public VideoPostDTO VideoPostDTO { get; set; }

        [Required]
        public IFormFile VideoFile { get; set; }

        public IFormFile PreviewFile { get; set; }
    }
}
