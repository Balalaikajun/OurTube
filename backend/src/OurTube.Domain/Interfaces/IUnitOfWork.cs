using Microsoft.AspNetCore.Identity;
using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IPlaylistRepository Playlists { get; }
        IRepository<PlaylistElement> PlaylistElements { get; }
        IApplicationUserRepository ApplicationUsers { get; }
        IRepository<Subscription> Subscriptions { get; }
        IRepository<IdentityUser> IdentityUsers { get; }
        IVideoRepository Videos { get; }
        IRepository<VideoVote> VideoVotes { get; }
        IViewRepository Views { get; }
        ICommentRepository Comments { get; }
        IRepository<CommentVote> CommentVoices { get; }
        IRepository<Tag> Tags { get; }
        IRepository<VideoTags> VideoTags { get; }
        

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
