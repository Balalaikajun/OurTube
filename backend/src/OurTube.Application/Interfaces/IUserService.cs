using OurTube.Application.DTOs.ApplicationUser;

namespace OurTube.Application.Interfaces;

public interface IUserService
{
    Task<ApplicationUserDto> UpdateUserAsync(ApplicationUserPatchDto patchDto, Guid userId);
    Task<ApplicationUserDto> GetUserAsync(Guid userId);
}