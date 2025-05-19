using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces;

public interface ICommentVoteRepository: IRepository<CommentVote>
{
    Task<IEnumerable<CommentVote>> GetByUserIdAndCommentIdsAsync(string userId, IEnumerable<int> commentIds);
}