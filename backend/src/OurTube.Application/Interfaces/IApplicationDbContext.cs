using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using IdentityUser = OurTube.Domain.Entities.IdentityUser;

namespace OurTube.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; set; }
    DbSet<IdentityUser> IdentityUsers { get; set; }
    DbSet<UserAvatar> UserAvatars { get; set; }
    DbSet<Video> Videos { get; set; }
    DbSet<VideoPreview> VideoPreviews { get; set; }
    DbSet<VideoVote> VideoVotes { get; set; }

    DbSet<Playlist> Playlists { get; set; }
    DbSet<PlaylistElement> PlaylistElements { get; set; }
    DbSet<Comment> Comments { get; set; }
    DbSet<CommentVote> CommentVotes { get; set; }
    DbSet<Subscription> Subscriptions { get; set; }
    DbSet<VideoView> Views { get; set; }
    DbSet<Tag> Tags { get; set; }
    DbSet<VideoTags> VideoTags { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}