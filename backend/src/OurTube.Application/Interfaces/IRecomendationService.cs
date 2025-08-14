using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;

namespace OurTube.Application.Interfaces;

public interface IRecomendationService
{
    Task<ListReply<MinVideo>> GetRecommendationsAsync(Guid? userId, Guid sessionId, GetQueryParameters parameters);

    Task<ListReply<MinVideo>> GetRecommendationsForVideoAsync(Guid videoId, Guid? userId, Guid sessionId,
        GetQueryParameters parameters);
}