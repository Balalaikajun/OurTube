using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public ApplicationUserRepository(ApplicationDbContext context)
            : base(context) { }

        public ApplicationUser? GetWithAvatar(string userId)
        {
            return ApplicationDbContext.ApplicationUsers
                .Include(au => au.UserAvatar)
                .FirstOrDefault(au => au.Id == userId);
        }
    }
}
