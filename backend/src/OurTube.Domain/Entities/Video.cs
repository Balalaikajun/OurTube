using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    public class Video
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(150)]
        [Required]
        public string Title { get; set; }

        [MaxLength(5000)]
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int LikesCount { get; set; } = 0;

        [Required]
        public int DislikeCount { get; set; } = 0;

        [Required]
        public int CommentsCount { get; set; } = 0;

        [Required]
        public int ViewsCount { get; set; } = 0;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        //Navigation
        [Required]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public VideoPreview VideoPreview { get; set; }
        [Required]
        public VideoSource VideoSource { get; set; }
        public ICollection<VideoPlaylist> Files { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PlaylistElement> Playlists { get; set;}
    }
}
