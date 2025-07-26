using OurTube.Application.Replies.Comment;
using OurTube.Application.Replies.Common;
using OurTube.Application.Requests.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentRecommendationService
{
    Task<ListReply<Comment>> GetCommentsWithLimitAsync(GetCommentRequest commentRequest);
}