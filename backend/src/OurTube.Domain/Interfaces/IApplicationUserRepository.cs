using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{

    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetWithAvatarAsync(string userId);
    }
}
