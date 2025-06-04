using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>,IApplicationDbContext
{
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

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
    
    private readonly IMediator _mediator;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // IdentityUser
        modelBuilder.Entity<IdentityUser>()
            .ToTable(nameof(IdentityUser));

        // ApplicationUser 
        modelBuilder.Entity<ApplicationUser>()
            .ToTable(nameof(ApplicationUser));

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.SubscribedTo)
            .WithOne(s => s.Subscriber)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(u => u.Subscribers)
            .WithOne(s => s.SubscribedTo)
            .HasForeignKey(s => s.SubscribedToId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApplicationUser>()
            .HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);


        // Subscription
        modelBuilder.Entity<Subscription>()
            .HasKey(s => new { s.SubscriberId, s.SubscribedToId }); // Установка составного ключа

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Subscriber)
            .WithMany(u => u.SubscribedTo)
            .HasForeignKey(s => s.SubscriberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.SubscribedTo)
            .WithMany(u => u.Subscribers)
            .HasForeignKey(s => s.SubscribedToId)
            .OnDelete(DeleteBehavior.Cascade);

        // Video
        modelBuilder.Entity<Video>()
            .HasOne(v => v.Preview)
            .WithOne(vp => vp.Video)
            .HasForeignKey<VideoPreview>(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Video>()
            .HasOne(v => v.Source)
            .WithOne(vp => vp.Video)
            .HasForeignKey<VideoSource>(vp => vp.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Video>()
            .Property(v => v.Duration)
            .HasColumnType("interval");

        // Views
        modelBuilder.Entity<VideoView>()
            .ToTable(nameof(VideoView));

        modelBuilder.Entity<VideoView>(entity =>
        {
            entity.Property(vv => vv.WhatchTime)
                .HasColumnType("interval");
                
            entity.Property(vv => vv.EndTime)
                .IsRequired()
                .HasColumnType("interval");
        });
                

        // VideoTags
        modelBuilder.Entity<VideoTags>()
            .HasOne(vt => vt.Video)
            .WithMany(v => v.Tags)
            .HasForeignKey(vt => vt.VideoId)
            .OnDelete(DeleteBehavior.Cascade);
            
        modelBuilder.Entity<VideoTags>()
            .HasOne(vt => vt.Tag)
            .WithMany()
            .HasForeignKey(vt => vt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
                

        // Playlist
        modelBuilder.Entity<Playlist>()
            .HasMany(p => p.PlaylistElements)
            .WithOne(pe => pe.Playlist)
            .HasForeignKey(pe => pe.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        // PlaylistElement
        modelBuilder.Entity<PlaylistElement>()
            .HasOne(pe => pe.Video)
            .WithMany()
            .HasForeignKey(pe => pe.VideoId)
            .OnDelete(DeleteBehavior.Cascade);

    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var domainEntities = this.ChangeTracker.Entries<BaseEntity>()
            .Where(be => be.Entity.DomainEvents.Count != 0)
            .Select(be => be.Entity)
            .ToList();
            
        var domainEvents = domainEntities.SelectMany(be => be.DomainEvents).ToList();
            
        var result = await base.SaveChangesAsync(cancellationToken);
            
        domainEntities.ForEach(be => be.ClearDomainEvents());

        await Task.WhenAll(domainEvents.Select(e=> _mediator.Publish(e,cancellationToken)));
            
        return result;
    }
}