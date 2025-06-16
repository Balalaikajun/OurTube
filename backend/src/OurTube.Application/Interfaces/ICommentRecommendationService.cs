using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentRecommendationService
{
    Task<PagedCommentDto> GetCommentsWithLimitAsync(
        int videoId, int limit, int after,
        string sessionId, string? userId,
        int? parentId = null,
        bool reload = false);
}