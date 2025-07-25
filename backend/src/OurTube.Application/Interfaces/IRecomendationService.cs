using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<PagedVideoDto> GetRecommendationsAsync(Guid? userId, Guid sessionId,
        int limit, int after,
        bool reload = false);
    Task<PagedVideoDto> GetRecommendationsForVideoAsync( Guid videoId,
        Guid? userId, Guid sessionId,
        int limit, int after,
        bool reload = false);
}