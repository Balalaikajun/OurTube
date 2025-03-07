using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Video
{
    public  class VideoUploadDTO
    {
        [Required]
        public VideoPostDTO VideoPostDTO { get; set; }

        [Required]
        public IFormFile VideoFile { get; set; }

        public IFormFile PreviewFile { get; set; }
    }
}
