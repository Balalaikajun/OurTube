using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime Created { get; set; }
        [MaxLength(125)]
        public string? AvatarPath { get; set; }

        //Navigation
        public ICollection<Subscription> Subscribers { get; set; }
        public ICollection<Subscription> SubscribedTo { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<View> Views { get; set; }
        public ICollection<Vote> Votes { get; set; }



    }
}
