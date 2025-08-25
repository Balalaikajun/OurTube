using Microsoft.AspNetCore.Http;
using OurTube.Application.Replies.UserAvatar;

namespace OurTube.Application.Interfaces;

public interface IUserAvatarService
{
    Task<UserAvatar> CreateOrUpdateUserAvatarAsync(IFormFile image, Guid userId);
    Task DeleteUserAvatarAsync(Guid userId);
}