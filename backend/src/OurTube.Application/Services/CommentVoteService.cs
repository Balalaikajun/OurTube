using Microsoft.EntityFrameworkCore;
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

    public async Task SetAsync(int commentId, string userId, bool type)
    {
        var comment = await _dbContext.Comments
            .FindAsync(commentId);

        if (comment == null)
            throw new InvalidOperationException("Комментарий не найден");

        if (comment.IsDeleted)
            throw new InvalidOperationException("Комментарий удалён");

        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var vote = await _dbContext.CommentVotes.FindAsync(commentId, userId);

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

    public async Task DeleteAsync(int commentId, string userId)
    {
        var comment = await _dbContext.Comments
            .FindAsync(commentId);


        if (comment == null)
            throw new InvalidOperationException("Комментарий не найдено");

        if (comment.IsDeleted)
            throw new InvalidOperationException("Комментарий удалён");

        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var vote = await _dbContext.CommentVotes.FindAsync(commentId, userId);

        if (vote == null)
            return;

        if (vote.Type)
            comment.LikesCount--;
        else
            comment.DislikesCount--;

        _dbContext.CommentVotes.Remove(vote);

        await _dbContext.SaveChangesAsync();
    }
}