using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Video
{
    public class VideoUploadDto
    {
        [Required]
        public VideoPostDto VideoPostDto { get; set; }

        [Required]
        public IFormFile VideoFile { get; set; }

        public IFormFile PreviewFile { get; set; }
    }
}
