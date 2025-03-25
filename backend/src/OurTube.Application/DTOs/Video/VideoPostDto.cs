using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Video
{
    public class VideoPostDto
    {
        [MaxLength(150, ErrorMessage = "Название видео не должно превышать 150 символов")]
        [Required]
        public string Title { get; set; }
        [MaxLength(5000, ErrorMessage = "Описание не должно превышать 5000 символов")]
        [Required]
        public string Description { get; set; } = string.Empty;
        
        public IEnumerable<string> Tags { get; set; } = new List<string>();
    }
}
