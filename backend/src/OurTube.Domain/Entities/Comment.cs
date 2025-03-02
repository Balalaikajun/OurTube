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
        [MaxLength(5000)]
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;
        [Required]
        public DateTime Updated { get; set; } = DateTime.Now;
        public int? CommentId { get; set; }

        //Navigation
        public ApplicationUser User { get; set; }
        public Comment? Parent { get; set; }
        public ICollection<Comment> Childs { get; set; }

    }
}
