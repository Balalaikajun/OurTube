using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class VideoRepository : Repository<Video>, IVideoRepository
    {
        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public VideoRepository(ApplicationDbContext context)
            : base(context) { }

        public async Task<Video> GetFullVideoDataAsync(int videoId)
        {
            return await ApplicationDbContext.Videos
                .Include(v => v.Preview)
                .Include(v => v.Files)
                .Include(v => v.User)
                    .ThenInclude(u => u.UserAvatar)
                .FirstOrDefaultAsync(v => v.Id == videoId);
        }

        public async Task<Video> GetMinVideoDataAsync(int videoId)
        {
            return await ApplicationDbContext.Videos
                .Include(v => v.Preview)
                .Include(v => v.User)
                    .ThenInclude(u => u.UserAvatar)
                .FirstOrDefaultAsync(v => v.Id == videoId);
        }
    }
}
