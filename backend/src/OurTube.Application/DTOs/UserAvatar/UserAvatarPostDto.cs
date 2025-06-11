using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.DTOs.UserAvatar;

public class UserAvatarPostDto
{
    [Required]
    public IFormFile Image { get; set; }
}