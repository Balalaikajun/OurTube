using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Other
{
    public class ApplicationUserManager : UserManager<IdentityUser>
    {
        public ApplicationDbContext _applicationDbContext;
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
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _applicationDbContext = applicationDbContext;
        }

        public override async Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            var result = await base.CreateAsync(user, password);

            if (result.Succeeded)
            {

                _applicationDbContext.ApplicationUsers.Add(new ApplicationUser()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Playlists = [
                        new Playlist()
                        {
                        Title ="Понравившееся"
                        }]
                });

                try
                {
                    await _applicationDbContext.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    return IdentityResult.Failed(new IdentityError()
                    {
                        Code = "ApplicationUserCreateError",
                        Description = $"Ошибка при сохранении данных ApplicationUser: {ex.Message}"
                    });
                }
            }
            return result;
        }

    }
}
