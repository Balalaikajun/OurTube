using OurTube.Application.DTOs.UserAvatar;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.ApplicationUser
{
    public class ApplicationUserDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool IsSubscribed { get; set; }
        public int SubscribersCount { get; set; } = 0;
        public UserAvatarDTO? UserAvatar { get; set; }
    }
}
