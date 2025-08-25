using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using IdentityUser = OurTube.Domain.Entities.IdentityUser;

namespace OurTube.Infrastructure.Services;

public class ApplicationUserManager : UserManager<IdentityUser>
{
    public ApplicationDbContext ApplicationDbContext;

    public ApplicationUserManager(
        IUserStore<IdentityUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<IdentityUser> passwordHasher,
        IEnumerable<IUserValidator<IdentityUser>> userValidators,
        IEnumerable<IPasswordValidator<IdentityUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<IdentityUser>> logger,
        ApplicationDbContext applicationDbContext)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors,
            services, logger)
    {
        ApplicationDbContext = applicationDbContext;
    }

    public override async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
    {
        var result = await base.CreateAsync(user, password);

        if (!result.Succeeded)
            return result;

        ApplicationDbContext.ApplicationUsers.Add(new ApplicationUser
        {
            Id = user.Id,
            UserName = user.UserName
        });

        ApplicationDbContext.Playlists.Add(
            new Playlist
            {
                ApplicationUserId = user.Id,
                Title = "Понравившееся",
                IsSystem = true
            });

        ApplicationDbContext.Playlists.Add(
            new Playlist
            {
                ApplicationUserId = user.Id,
                Title = "Смотреть позже",
                IsSystem = true
            });

        try
        {
            await ApplicationDbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return IdentityResult.Failed(new IdentityError
            {
                Code = "ApplicationUserCreateError",
                Description = $"Ошибка при сохранении данных ApplicationUser: {ex.Message}"
            });
        }

        return result;
    }
}