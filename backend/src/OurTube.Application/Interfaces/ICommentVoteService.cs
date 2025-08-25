namespace OurTube.Application.Interfaces;

public interface ICommentVoteService
{
    Task SetAsync(Guid commentId, Guid userId, bool type);
    Task DeleteAsync(Guid commentId, Guid userId);
}