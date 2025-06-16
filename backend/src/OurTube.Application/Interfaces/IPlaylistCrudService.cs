using OurTube.Application.DTOs.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistCrudService
{
    Task<PlaylistMinGetDto> CreateAsync(PlaylistPostDto playlistDto, string userId);
    Task UpdateAsync(PlaylistPatchDto patchDto, int playlistId, string userId);
    Task DeleteAsync(int playlistId, string userId);
    Task AddVideoAsync(int playlistId, int videoId, string userId);

    Task RemoveVideoAsync(
        int playlistId,
        int videoId,
        string userId,
        bool suppressDomainEvent = false);
}