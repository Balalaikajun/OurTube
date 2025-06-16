using OurTube.Application.DTOs.Playlist;

namespace OurTube.Application.Interfaces;

public interface IPlaylistQueryService
{
    Task<PlaylistGetDto> GetWithLimitAsync(int playlistId, string userId, int limit, int after);
    Task<IEnumerable<PlaylistMinGetDto>> GetUserPlaylistsAsync(string userId);
    Task<IEnumerable<PlaylistForVideoGetDto>> GetUserPlaylistsForVideoAsync(string userId, int videoId);
    Task<PlaylistMinGetDto> GetLikedPlaylistAsync(string userId);
}