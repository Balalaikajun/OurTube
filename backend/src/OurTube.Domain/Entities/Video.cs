using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public ApplicationUser User { get; set; }
        [Required]
        public VideoPreview Preview { get; set; }
        [Required]
        public VideoSource Source { get; set; }
        public ICollection<VideoPlaylist> Files { get; set; }
        public ICollection<VideoVote> Votes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<View> Views { get; set; }
    }
}
