using OurTube.Application.Replies.Comment;
using OurTube.Application.Requests.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentCrudService
{
    Task<Comment> CreateAsync(Guid userId, Guid videoId, PostCommentRequest postCommentDto);
    Task UpdateAsync(Guid id, UpdateCommentRequest request);
    Task DeleteAsync(Guid commentId);
}