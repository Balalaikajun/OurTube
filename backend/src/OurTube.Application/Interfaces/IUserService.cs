using OurTube.Application.Replies.ApplicationUser;
using OurTube.Application.Requests.ApplicationUser;

namespace OurTube.Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUser> UpdateUserAsync(PatchApplicationUserRequest patchDto, Guid userId);
    Task<ApplicationUser> GetUserAsync(Guid userId);
}