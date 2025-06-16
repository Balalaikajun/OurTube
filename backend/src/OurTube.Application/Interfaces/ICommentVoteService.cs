namespace OurTube.Application.Interfaces;

public interface ICommentVoteService
{
    Task SetAsync(int commentId, string userId, bool type);
    Task DeleteAsync(int commentId, string userId);
}