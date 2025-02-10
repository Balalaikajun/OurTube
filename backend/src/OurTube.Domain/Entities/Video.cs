using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
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
        public int UserId { get; set; }

        //Navigation
        public User User { get; set; }
        public ICollection<VideoFile> Files { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}
