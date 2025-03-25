using MediatR;
using Microsoft.AspNetCore.Identity;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Persistence.Repositories;

namespace OurTube.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
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
        public IRepository<Tag> Tags { get; private set; }
        public IRepository<VideoTags> VideoTags { get; private set; }

        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UnitOfWork(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
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
            Tags = new Repository<Tag>(_context);
            VideoTags = new Repository<VideoTags>(_context);
        }
        

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            
            
            var domainEntities = _context.ChangeTracker.Entries<BaseEntity>()
                .Where(be => be.Entity.DomainEvents.Count != 0)
                .Select(be => be.Entity)
                .ToList();
            
            var domainEvents = domainEntities.SelectMany(be => be.DomainEvents).ToList();
            
            var result = await _context.SaveChangesAsync(cancellationToken);
            
            domainEntities.ForEach(be => be.ClearDomainEvents());

            await Task.WhenAll(domainEvents.Select(e=> _mediator.Publish(e,cancellationToken)));
            
            return result;
        }
    }
}
