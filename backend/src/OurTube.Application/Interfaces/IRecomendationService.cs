using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after, string? userId);
}