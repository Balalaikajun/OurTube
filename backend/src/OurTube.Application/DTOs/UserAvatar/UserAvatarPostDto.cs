using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.DTOs.UserAvatar;

/// <summary>
/// Запрос на обновление аватара пользователя.
/// </summary>
public class UserAvatarPostDto
{
    /// <summary>
    /// Аватар пользователя.
    /// </summary>
    [Required] public IFormFile Image { get; set; }
}