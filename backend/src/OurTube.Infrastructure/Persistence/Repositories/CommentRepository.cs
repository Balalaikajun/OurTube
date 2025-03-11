using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public CommentRepository(ApplicationDbContext context)
        : base(context) { }

        public IEnumerable<Comment> GetWithLimit(int videoId, int limit, int after, int? parentId = null)
        {
            return ApplicationDbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Childs)
                    .ThenInclude(c => c.User)
                .Where(c => c.VideoId == videoId && c.ParentId == parentId)
                .OrderBy(c => c.LikesCount)
                .Skip(after)
                .Take(limit)
                .ToList();
        }
    }
}
