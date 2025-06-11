using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.Interfaces;

namespace OurTube.Application.Services;

public class UserService
{
    private readonly IApplicationDbContext _dbContext;

    public UserService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task UpdateUserAsync(ApplicationUserPatchDto patchDto, string userId)
    {
        var aUser = await _dbContext.ApplicationUsers.FindAsync(userId);

        if (aUser == null)
            throw new KeyNotFoundException("Пользователь не найден");

        var iUser = await _dbContext.IdentityUsers.FindAsync(userId);

        if (iUser == null)
            throw new KeyNotFoundException("Пользователь не найден");

        if (patchDto.UserName != null)
        {
            aUser.UserName = patchDto.UserName;
            iUser.UserName = patchDto.UserName;
        }

        await _dbContext.SaveChangesAsync();
    }
}