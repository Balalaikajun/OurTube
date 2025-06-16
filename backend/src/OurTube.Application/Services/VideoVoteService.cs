using Microsoft.EntityFrameworkCore;
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

    public async Task SetAsync(int videoId, string userId, bool type)
    {
        var video = await _dbContext.Videos.FindAsync(videoId);
        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var vote = await _dbContext.VideoVotes.FindAsync(videoId, userId);

        if (vote == null)
            _dbContext.VideoVotes.Add(new VideoVote(videoId, userId, type));
        else if (vote.Type != type)
            vote.Update(type);
        else
            return;


        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int videoId, string userId)
    {
        var video = await _dbContext.Videos.FindAsync(videoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var vote = await _dbContext.VideoVotes
            .FindAsync(videoId, userId);

        if (vote == null)
            return;

        vote.RemoveEvent();

        _dbContext.VideoVotes.Remove(vote);

        await _dbContext.SaveChangesAsync();
    }
}