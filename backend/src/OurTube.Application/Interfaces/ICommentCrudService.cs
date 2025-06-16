using OurTube.Application.DTOs.Comment;

namespace OurTube.Application.Interfaces;

public interface ICommentCrudService
{
    Task<CommentGetDto> CreateAsync(string userId, CommentPostDto postDto);
    Task UpdateAsync(string userId, CommentPatchDto postDto);
    Task DeleteAsync(int commentId, string userId);
}