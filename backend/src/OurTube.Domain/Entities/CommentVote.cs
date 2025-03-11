using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(CommentId), nameof(ApplicationUserId))]
    public class CommentVote
    {
        public int CommentId { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public bool Type { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        //Navigation
        public Comment Comment { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
