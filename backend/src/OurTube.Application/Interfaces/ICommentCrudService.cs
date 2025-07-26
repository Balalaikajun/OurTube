using OurTube.Application.Replies.Comment;
using OurTube.Application.Requests.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentCrudService
{
    Task<Comment> CreateAsync(Guid userId, PostCommentRequest postCommentDto);
    Task UpdateAsync(UpdateCommentRequest request);
    Task DeleteAsync(Guid commentId);
}