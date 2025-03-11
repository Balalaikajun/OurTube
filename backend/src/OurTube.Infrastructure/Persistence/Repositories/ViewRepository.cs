using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class ViewRepository : Repository<View>, IViewRepository
    {
        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public ViewRepository(ApplicationDbContext context)
        : base(context) { }

        public IEnumerable<View> GetHistoryWithLimit(string userId, int limit, int after)
        {
            return ApplicationDbContext.Views
                .Where(v => v.ApplicationUserId == userId)
                .OrderByDescending(v => v.DateTime)
                .Skip(after)
                .Take(limit);
        }
    }
}
