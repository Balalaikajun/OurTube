using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentRecommendationService
{
    Task<PagedCommentDto> GetCommentsWithLimitAsync(GetCommentsRequest request);
}