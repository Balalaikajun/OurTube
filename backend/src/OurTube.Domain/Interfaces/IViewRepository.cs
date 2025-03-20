using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IViewRepository : IRepository<VideoView>
    {
        Task<IEnumerable<VideoView>> GetHistoryWithLimitAsync(string userId, int limit, int after);
    }
}
