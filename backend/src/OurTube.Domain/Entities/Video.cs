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
        public int DeslikeCount { get; set; } = 0;

        [Required]
        public int CommentsCount { get; set; } = 0;

        [Required]
        public int ViewsCount { get; set; } = 0;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [MaxLength(125)]
        [Required]
        public string PreviewPath { get; set; }
        [MaxLength(125)]
        [Required]
        public string SourcePath { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }

        //Navigation
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<VideoPlaylist> Files { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<PlaylistElement> Playlists { get; set;}
    }
}
