using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class CommentVoteService : ICommentVoteService
{
    private readonly IApplicationDbContext _dbContext;

    public CommentVoteService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SetAsync(Guid commentId, Guid userId, bool type)
    {
        var comment = await _dbContext.Comments
            .FindAsync(commentId, true);

        await _dbContext.ApplicationUsers.EnsureExistAsync(userId);

        var vote = await _dbContext.CommentVotes
            .FirstOrDefaultAsync( cv => cv.CommentId == commentId && cv.ApplicationUserId == userId);

        if (vote == null)
        {
            _dbContext.CommentVotes.Add(new CommentVote
            {
                ApplicationUserId = userId,
                CommentId = commentId,
                Type = type
            });

            if (type)
                comment.LikesCount++;
            else
                comment.DislikesCount++;
        }
        else if (vote.Type != type)
        {
            vote.Type = type;

            if (type)
            {
                comment.DislikesCount--;
                comment.LikesCount++;
            }
            else
            {
                comment.DislikesCount++;
                comment.LikesCount--;
            }
        }
        else
        {
            return;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId, Guid userId)
    {
        var comment = await _dbContext.Comments
            .FindAsync(commentId, true);

       var vote =  await _dbContext.CommentVotes
            .GetAsync(cv => cv.CommentId == commentId && cv.ApplicationUserId == userId, true);

       if (vote.Type)
           comment.LikesCount--;
       else
           comment.DislikesCount--;

        vote.Delete();

        await _dbContext.SaveChangesAsync();
    }
}