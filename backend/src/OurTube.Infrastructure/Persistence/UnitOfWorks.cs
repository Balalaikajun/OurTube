using Microsoft.AspNetCore.Identity;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Persistence.Repositories;

namespace OurTube.Infrastructure.Persistence
{
    public class UnitOfWorks : IUnitOfWorks
    {
        public IPlaylistRepository Playlists { get; private set; }
        public IRepository<PlaylistElement> PlaylistElements { get; private set; }
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public IRepository<Subscription> Subscriptions { get; private set; }
        public IRepository<IdentityUser> IdentityUsers { get; private set; }
        public IVideoRepository Videos { get; private set; }
        public IRepository<VideoVote> VideoVotes { get; private set; }
        public IViewRepository Views { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public IRepository<CommentVote> CommentVoices { get; private set; }

        private readonly ApplicationDbContext _context;

        public UnitOfWorks(ApplicationDbContext context)
        {
            _context = context;
            Playlists = new PlaylistRepository(_context);
            PlaylistElements = new Repository<PlaylistElement>(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
            Subscriptions = new Repository<Subscription>(_context);
            IdentityUsers = new Repository<IdentityUser>(_context);
            Videos = new VideoRepository(_context);
            VideoVotes = new Repository<VideoVote>(_context);
            Views = new ViewRepository(_context);
            Comments = new CommentRepository(_context);
            CommentVoices = new Repository<CommentVote>(_context);
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
