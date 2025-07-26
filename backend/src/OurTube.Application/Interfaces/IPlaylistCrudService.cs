using OurTube.Application.Replies.Playlist;
using OurTube.Application.Requests.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistCrudService
{
    Task<Playlist> CreateAsync(PostPlaylistRequest playlistDto, Guid userId);
    Task UpdateAsync(UpdatePlaylistRequest patchDto, Guid playlistId);
    Task DeleteAsync(Guid id);
    Task AddVideoAsync(Guid playlistId, Guid videoId);

    Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        bool suppressDomainEvent = false);
}