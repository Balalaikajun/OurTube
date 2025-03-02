using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs
{
    public class ApplicationUserDTO
    {
        [Required]
        public int SubscribersCount { get; set; } = 0;
        public UserAvatarDTO? UserAvatar { get; set; }
    }
}
