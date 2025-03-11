using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;

namespace OurTube.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistElement> PlaylistElements { get; set; }
        public DbSet<Bucket> Buckets { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentVote> CommentVotes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<View> Views { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
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

            // Views
            modelBuilder.Entity<View>()
                .ToTable(nameof(View));



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
    }
}
