using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<IEnumerable<VideoMinGetDto>> GetRecommendationsAsync(string? userId, string sessionId,
        int limit, int after,
        bool reload = false);
}