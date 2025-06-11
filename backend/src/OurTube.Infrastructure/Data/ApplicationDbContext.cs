using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<UserAvatar> UserAvatars { get; set; }

    public DbSet<IdentityUser> IdentityUsers
    {
        get => base.Users;
        set => base.Users = value;
    }

    public DbSet<Video> Videos { get; set; }
    public DbSet<VideoVote> VideoVotes { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistElement> PlaylistElements { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentVote> CommentVotes { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<VideoView> Views { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<VideoTags> VideoTags { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var domainEntities = ChangeTracker.Entries<BaseEntity>()
            .Where(be => be.Entity.DomainEvents.Count != 0)
            .Select(be => be.Entity)
            .ToList();

        var domainEvents = domainEntities.SelectMany(be => be.DomainEvents).ToList();

        var result = await base.SaveChangesAsync(cancellationToken);

        domainEntities.ForEach(be => be.ClearDomainEvents());

        await Task.WhenAll(domainEvents.Select(e => _mediator.Publish(e, cancellationToken)));

        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // IdentityUser
        modelBuilder.Entity<IdentityUser>()
            .ToTable(nameof(IdentityUser));
    }
}