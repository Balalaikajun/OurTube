using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class ApplicationUser 
    {
        [Key]
        public string Id{ get; set; }
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
