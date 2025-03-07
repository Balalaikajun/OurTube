using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class ApplicationUser 
    {
        [Key]
        public string Id{ get; set; }
        [MaxLength(256)]
        [Required]
        public string UserName { get; set; }
        [Required]
        public int SubscribersCount { get; set; } = 0;
        [Required]
        public int SubscribedToCount { get; set; } = 0;
        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        //Navigation
        public ICollection<Subscription> Subscribers { get; set; }
        public ICollection<Subscription> SubscribedTo { get; set; }
        public ICollection<Video> Videos { get; set; }
        public ICollection<View> Views { get; set; }
        public ICollection<VideoVote> VideoVotes { get; set; }
        public ICollection<CommentVote> CommentVotes { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public UserAvatar? UserAvatars { get; set; }


    }
}
