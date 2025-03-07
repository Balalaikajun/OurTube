using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Video
{
    public class VideoPostDTO
    {
        [MaxLength(150, ErrorMessage ="Название видео не должно превышать 150 символов")]
        [Required]
        public string Title { get; set; }
        [MaxLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
