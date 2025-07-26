using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Playlist;
using OurTube.Application.Replies.PlaylistElement;

namespace OurTube.Application.Interfaces;

public interface IPlaylistQueryService
{
    Task<Playlist> GetMinById(Guid id);
    Task<ListReply<PlaylistElement>> GetElements(Guid playlistId, Guid userId, int limit, int after);
    Task<IEnumerable<Playlist>> GetUserPlaylistsAsync(Guid userId);
    Task<IEnumerable<PlaylistForVideo>> GetUserPlaylistsForVideoAsync(Guid userId, Guid videoId);
    Task<Playlist> GetLikedPlaylistAsync(Guid userId);
}