using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class ApplicationUser 
    {
        [Key]
        public string Id{ get; set; }
        [Required]
        public int SubscribersCount { get; set; }
        [Required]
        public int SubscribedToCount { get; set; }
        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        //Navigation
        public ICollection<Subscription> Subscribers { get; set; }
        public ICollection<Subscription> SubscribedTo { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<View> Views { get; set; }
        public ICollection<Vote> Votes { get; set; }
        public UserAvatar? UserAvatars { get; set; }


    }
}
