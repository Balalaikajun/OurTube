using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class ViewService : IViewService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVideoService _videoService;

    public ViewService(IApplicationDbContext dbContext, IVideoService videoService, IMapper mapper)
    {
        _dbContext = dbContext;
        _videoService = videoService;
        _mapper = mapper;
    }

    public async Task AddVideoAsync(int videoId, string userId, TimeSpan endTime)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var video = await _dbContext.Videos.FindAsync(videoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        var view = await _dbContext.Views.FindAsync(videoId, userId);

        if (view != null)
        {
            view.DateTime = DateTime.UtcNow;
            view.EndTime = endTime;
        }
        else
        {
            view = new VideoView
            {
                ApplicationUserId = userId,
                VideoId = videoId,
                EndTime = endTime,
                DateTime = DateTime.UtcNow
            };

            _dbContext.Views.Add(view);
            video.ViewsCount++;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveVideoAsync(int videoId, string userId)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        if (!await _dbContext.Videos.AnyAsync(v => v.Id == videoId))
            throw new InvalidOperationException("Видео не найдено");

        var view = await _dbContext.Views.FindAsync(videoId, userId);

        if (view == null)
            return;

        _dbContext.Views.Remove(view);

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearHistoryAsync(string userId)
    {
        var applicationUser = await _dbContext.ApplicationUsers.FindAsync(userId);

        if (applicationUser == null)
            throw new InvalidOperationException("Пользователь не найден");

        var views = await _dbContext.Views
            .Where(vv => vv.ApplicationUserId == applicationUser.Id)
            .ToListAsync();

        _dbContext.Views.RemoveRange(views);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedVideoDto> GetWithLimitAsync(string userId, int limit, int after)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var videos = await _dbContext.Views
            .Where(v => v.ApplicationUserId == userId)
            .OrderByDescending(v => v.DateTime)
            .Skip(after)
            .Take(limit + 1)
            .Select(v => v.Video)
            .ProjectToMinDto(_mapper, userId)
            .ToListAsync();

        return new PagedVideoDto
        {
            Videos = videos.Take(limit),
            NextAfter = after + limit,
            HasMore = videos.Count > limit
        };
    }
}