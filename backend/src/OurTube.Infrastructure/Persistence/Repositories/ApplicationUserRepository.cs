using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public ApplicationUserRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<ApplicationUser?> GetWithAvatarAsync(string userId)
        {
            return await ApplicationDbContext.ApplicationUsers
                .Include(au => au.UserAvatar)
                .FirstOrDefaultAsync(au => au.Id == userId);
        }
    }
}
