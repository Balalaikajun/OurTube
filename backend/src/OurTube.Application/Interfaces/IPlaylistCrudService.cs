using OurTube.Application.DTOs.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistCrudService
{
    Task<PlaylistMinGetDto> CreateAsync(PlaylistPostDto playlistDto, Guid userId);
    Task UpdateAsync(PlaylistPatchDto patchDto, Guid playlistId);
    Task DeleteAsync(Guid id);
    Task AddVideoAsync(Guid playlistId, Guid videoId);

    Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        bool suppressDomainEvent = false);
}