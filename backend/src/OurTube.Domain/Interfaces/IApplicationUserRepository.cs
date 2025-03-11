using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{

    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        ApplicationUser? GetWithAvatar(string userId);
    }
}
