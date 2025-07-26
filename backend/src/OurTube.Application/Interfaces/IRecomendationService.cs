using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Recommendation;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<ListReply<MinVideo>> GetRecommendationsAsync(GetRecommendationsRequest request);
    Task<ListReply<MinVideo>> GetRecommendationsForVideoAsync(GetRecommendationsForVideoRequest request);
}