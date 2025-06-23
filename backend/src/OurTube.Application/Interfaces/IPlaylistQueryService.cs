using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;

namespace OurTube.Application.Interfaces;

public interface IPlaylistQueryService
{
    Task<PagedDto<PlaylistElementGetDto>> GetElements(int playlistId, string userId, int limit, int after);
    Task<IEnumerable<PlaylistMinGetDto>> GetUserPlaylistsAsync(string userId);
    Task<IEnumerable<PlaylistForVideoGetDto>> GetUserPlaylistsForVideoAsync(string userId, int videoId);
    Task<PlaylistMinGetDto> GetLikedPlaylistAsync(string userId);
}