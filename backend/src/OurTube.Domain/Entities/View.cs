using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
{
    public class View
    {
        [Key]
        public int VideoId { get; set; }
        [Key]
        public int UserId{ get; set; }
        [Required]
        public long EndTime { get; set; }
        [Required]
        public DateTime DateTime { get; set; } = DateTime.Now;

        //Navigation
        public Video Video { get; set; }
        public User User { get; set; }  
    }
}
