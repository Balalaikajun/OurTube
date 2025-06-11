using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Views;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class ViewService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly VideoService _videoService;

    public ViewService(IApplicationDbContext dbContext, VideoService videoService, IMapper mapper)
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

    public async Task<List<ViewGetDto>> GetWithLimitAsync(string userId, int limit, int after)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var result = await _dbContext.Views
            .Where(v => v.ApplicationUserId == userId)
            .OrderByDescending(v => v.DateTime)
            .Skip(after)
            .Take(limit)
            .ProjectTo<ViewGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return result;
    }
}