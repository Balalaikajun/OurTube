using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces;

public interface IRecomendationRepository
{
    Task<IEnumerable<int>> GetIndexesPopularAsync();
    Task<IEnumerable<int>> GetIndexesByTagsAsync(string userId);
    Task<IEnumerable<int>> GetIndexesBySubscriptionAsync(string userId);
}