using OurTube.Application.DTOs.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistCrudService
{
    Task<PlaylistMinGetDto> CreateAsync(PlaylistPostDto playlistDto, Guid userId);
    Task UpdateAsync(PlaylistPatchDto patchDto, Guid playlistId, Guid userId);
    Task DeleteAsync(Guid playlistId, Guid userId);
    Task AddVideoAsync(Guid playlistId, Guid videoId, Guid userId);

    Task RemoveVideoAsync(
        Guid playlistId,
        Guid videoId,
        Guid userId,
        bool suppressDomainEvent = false);
}