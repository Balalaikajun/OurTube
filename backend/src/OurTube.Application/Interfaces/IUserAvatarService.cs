using Microsoft.AspNetCore.Http;
using OurTube.Application.DTOs.UserAvatar;

namespace OurTube.Application.Interfaces;

public interface IUserAvatarService
{
    Task<UserAvatarDto> CreateOrUpdateUserAvatarAsync(IFormFile image, string userId);
    Task DeleteUserAvatarAsync(string userId);
}