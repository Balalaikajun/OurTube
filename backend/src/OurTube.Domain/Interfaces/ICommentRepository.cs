using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        IEnumerable<Comment> GetWithLimit(int videoId, int limit, int after, int? parentId = null);
    }
}
