

using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
{
    public class User
    {
        [Key]
        public int Id {  get; set; }
        [MaxLength(150)]
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(60)]
        public string Password { get; set; }
        public DateTime Created { get; set; }
        [MaxLength(125)]
        public string? AvatarPath { get; set; }

        //Navigation
        public ICollection<User> Subscribers { get; set; }
        public ICollection<User> SubscribеTo { get; set; }
        public ICollection<Video> Videos{ get; set; }



    }
}
