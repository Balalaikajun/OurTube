using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
{
    public class Vote
    {
        [Key]
        public int VideoId { get; set; }
        [Key]
        public int UserId { get; set; }
        [Required]
        public bool Type {  get; set; }
        [Required]
        public DateTime Created {  get; set; }

        //Navigation
        public Video Video { get; set; }
        public User User { get; set; }
    }
}
