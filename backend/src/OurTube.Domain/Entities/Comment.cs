using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int VideoId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [MaxLength(1500)]
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime? Updated { get; set; }
        public int? ParentId { get; set; }
        [Required]
        public bool Edited { get; set; } = false;
        [Required]
        public int LikesCount { get; set; } = 0;
        [Required]
        public int DisLikesCount { get; set; } = 0;



        //Navigation
        public ApplicationUser User { get; set; }
        public Comment? Parent { get; set; }
        public ICollection<Comment> Childs { get; set; }
        public ICollection<CommentVote> Votes { get; set; }

    }
}
