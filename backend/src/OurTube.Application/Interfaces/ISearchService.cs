using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface ISearchService
{
    Task<PagedVideoDto> SearchVideos(
        string searchQuery,
        string? userId, string sessionId,
        int limit = 10, int after = 0,
        bool reload = true);
}