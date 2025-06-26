using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<PagedVideoDto> GetRecommendationsAsync(string? userId, string sessionId,
        int limit, int after,
        bool reload = false);
    Task<PagedVideoDto> GetRecommendationsForVideoAsync( int videoId,
        string? userId, string sessionId,
        int limit, int after,
        bool reload = false);
}