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

        public Video GetFullVideoData(int videoId)
        {
            return ApplicationDbContext.Videos
                .Include(v => v.Preview)
                .Include(v => v.Files)
                .Include(v => v.User)
                    .ThenInclude(u => u.UserAvatar)
                .FirstOrDefault(v => v.Id == videoId);
        }

        public Video GetMinVideoData(int videoId)
        {
            return ApplicationDbContext.Videos
                .Include(v => v.Preview)
                .Include(v => v.User)
                    .ThenInclude(u => u.UserAvatar)
                .FirstOrDefault(v => v.Id == videoId);
        }
    }
}
