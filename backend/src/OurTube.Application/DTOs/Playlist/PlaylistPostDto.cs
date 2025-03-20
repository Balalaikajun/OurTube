using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Playlist
{
    public class PlaylistPostDto
    {
        [MaxLength(150)]
        [Required]
        public string Title { get; set; }
        [MaxLength(5000)]
        [Required]
        public string Description { get; set; } = "";
    }
}
