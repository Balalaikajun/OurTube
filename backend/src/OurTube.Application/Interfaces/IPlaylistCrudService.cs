using OurTube.Application.Replies.Playlist;
using OurTube.Application.Requests.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistCrudService
{
    Task<Playlist> CreateAsync(Guid userId, PostPlaylistRequest request);
    Task UpdateAsync(Guid playlistId, UpdatePlaylistRequest request);
    Task DeleteAsync(Guid id);
    Task AddVideoAsync(Guid playlistId, Guid videoId);

    Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        bool suppressDomainEvent = false);
}