using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int VideoId{ get; set; }
        [Required]
        public int ApplicationUserId { get; set; }
        [MaxLength(5000)]
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Created {  get; set; }  =  DateTime.Now;
        [Required]
        public DateTime Updated { get; set; } = DateTime.Now;
        public int? CommentId { get; set; }

        //Navigation
        public Comment? CommentParent { get; set; }

    }
}
