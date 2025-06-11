using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.DTOs.Video;

public class VideoUploadDto
{
    [Required] public VideoPostDto VideoPostDto { get; set; }

    [Required] public IFormFile VideoFile { get; set; }

    public IFormFile PreviewFile { get; set; }
}