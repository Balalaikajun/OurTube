using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;

namespace OurTube.Application.Interfaces;

public interface IPlaylistQueryService
{
    Task<PlaylistMinGetDto> GetMinById(Guid id);
    Task<PagedDto<PlaylistElementGetDto>> GetElements(Guid playlistId, Guid userId, int limit, int after);
    Task<IEnumerable<PlaylistMinGetDto>> GetUserPlaylistsAsync(Guid userId);
    Task<IEnumerable<PlaylistForVideoGetDto>> GetUserPlaylistsForVideoAsync(Guid userId, Guid videoId);
    Task<PlaylistMinGetDto> GetLikedPlaylistAsync(Guid userId);
}