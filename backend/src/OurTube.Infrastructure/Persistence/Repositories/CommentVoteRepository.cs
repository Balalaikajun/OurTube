using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories;

public class CommentVoteRepository:Repository<CommentVote>,ICommentVoteRepository
{
    public ApplicationDbContext ApplicationDbContext
    {
        get { return Context as ApplicationDbContext; }
    }

    public CommentVoteRepository(ApplicationDbContext context)
        : base(context) { }
    
    public async Task<IEnumerable<CommentVote>> GetByUserIdAndCommentIdsAsync(string userId,
        IEnumerable<int> commentIds)
    {
        return await ApplicationDbContext.CommentVotes
            .Where(c => c.ApplicationUserId == userId && commentIds.Contains(c.CommentId))
            .ToListAsync();
    }

}