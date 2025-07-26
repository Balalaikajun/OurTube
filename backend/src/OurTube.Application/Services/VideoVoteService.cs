using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class VideoVoteService : IVideoVoteService
{
    private readonly IApplicationDbContext _dbContext;

    public VideoVoteService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SetAsync(Guid videoId, Guid userId, bool type)
    {
        await _dbContext.Videos.EnsureExistAsync(videoId);

        await _dbContext.ApplicationUsers.EnsureExistAsync(userId);

        var vote = await _dbContext.VideoVotes.FindAsync(videoId, userId);

        if (vote == null)
            _dbContext.VideoVotes.Add(new VideoVote(videoId, userId, type));
        else if (vote.Type != type)
            vote.Update(type);
        else
            return;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid videoId, Guid userId)
    {
        await _dbContext.Videos.EnsureExistAsync(videoId);

        await _dbContext.ApplicationUsers.EnsureExistAsync(userId);

        var vote = await _dbContext.VideoVotes
            .GetAsync(vv => vv.VideoId == videoId && vv.ApplicationUserId == userId, true);

        vote.RemoveEvent();

        vote.Delete();

        await _dbContext.SaveChangesAsync();
    }
}