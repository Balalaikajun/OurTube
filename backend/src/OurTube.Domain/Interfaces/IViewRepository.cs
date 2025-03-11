using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IViewRepository : IRepository<View>
    {
        IEnumerable<View> GetHistoryWithLimit(string userId, int limit, int after);
    }
}
