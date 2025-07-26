using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace OurTube.Application.Requests.UserAvatar;

/// <summary>
/// Запрос на обновление аватара пользователя.
/// </summary>
public class PostUserAvatarRequest
{
    /// <summary>
    /// Аватар пользователя.
    /// </summary>
    [Required]
    public required IFormFile Image { get; set; }
}