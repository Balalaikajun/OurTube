using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentRecommendationService
{
    Task<PagedCommentDto> GetCommentsWithLimitAsync(
        Guid videoId, int limit, int after,
        Guid sessionId, Guid? userId,
        Guid? parentId = null,
        bool reload = false);
}