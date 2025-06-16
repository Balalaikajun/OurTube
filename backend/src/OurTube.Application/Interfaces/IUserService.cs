using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUserDto> UpdateUserAsync(ApplicationUserPatchDto patchDto, string userId);
    Task<ApplicationUserDto> GetUserAsync(string userId);
}