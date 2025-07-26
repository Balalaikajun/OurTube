using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentCrudService
{
    Task<CommentGetDto> CreateAsync(Guid userId, CommentPostDto postDto);
    Task UpdateAsync(CommentPatchDto postDto);
    Task DeleteAsync(Guid commentId);
}