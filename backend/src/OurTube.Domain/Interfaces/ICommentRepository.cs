using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetWithLimitAsync(int videoId, int limit, int after, int? parentId = null);
    }
}
