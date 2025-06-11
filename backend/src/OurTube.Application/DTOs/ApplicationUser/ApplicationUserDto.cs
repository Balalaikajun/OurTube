using System.ComponentModel.DataAnnotations;
using OurTube.Application.DTOs.UserAvatar;

namespace OurTube.Application.DTOs.ApplicationUser;

public class ApplicationUserDto
{
    [Required] public string Id { get; set; }

    [Required] public string UserName { get; set; }

    [Required] public bool IsSubscribed { get; set; }

    public int SubscribersCount { get; set; } = 0;
    public UserAvatarDto? UserAvatar { get; set; }
}