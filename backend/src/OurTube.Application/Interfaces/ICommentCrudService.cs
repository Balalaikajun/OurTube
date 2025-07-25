using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentCrudService
{
    Task<CommentGetDto> CreateAsync(Guid userId, CommentPostDto postDto);
    Task UpdateAsync(Guid userId, CommentPatchDto postDto);
    Task DeleteAsync(Guid commentId, Guid userId);
}