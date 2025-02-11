using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VideoId { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [MaxLength(5000)]
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        [Required]
        public DateTime Updated { get; set; } = DateTime.Now;
        public int? CommentId { get; set; }

        //Navigation
        public Comment? Parent { get; set; }
        public ICollection<Comment> Childs { get; set; }

    }
}
