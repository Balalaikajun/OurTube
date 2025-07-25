using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface ISearchService
{
    Task<PagedVideoDto> SearchVideos(
        string searchQuery,
        Guid? userId, Guid sessionId,
        int limit = 10, int after = 0,
        bool reload = true);
}