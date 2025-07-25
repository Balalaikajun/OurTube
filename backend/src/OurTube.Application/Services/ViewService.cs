using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.Views;
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

    public async Task AddVideoAsync(ViewPostDto dto, Guid userId)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var video = await _dbContext.Videos.FindAsync(dto.VideoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        var view = await _dbContext.Views.FindAsync(dto.VideoId, userId);

        if (view != null)
        {
            view.EndTime = dto.EndTime;
        }
        else
        {
            view = new VideoView
            {
                ApplicationUserId = userId,
                VideoId = dto.VideoId,
                EndTime = dto.EndTime,
            };

            _dbContext.Views.Add(view);
            video.ViewsCount++;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveVideoAsync(Guid videoId, Guid userId)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        if (!await _dbContext.Videos.AnyAsync(v => v.Id == videoId))
            throw new InvalidOperationException("Видео не найдено");

        var view = await _dbContext.Views.FindAsync(videoId, userId);

        if (view == null)
            return;

        view.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearHistoryAsync(Guid userId)
    {
        var applicationUser = await _dbContext.ApplicationUsers.FindAsync(userId);

        if (applicationUser == null)
            throw new InvalidOperationException("Пользователь не найден");

        var views = await _dbContext.Views
            .Where(vv => vv.ApplicationUserId == applicationUser.Id)
            .ToListAsync();

        foreach (var view in views)
        {
            view.Delete();
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedVideoDto> GetWithLimitAsync(Guid userId, int limit, int after, string? query)
    {
        if (!await _dbContext.ApplicationUsers.AnyAsync(u => u.Id == userId))
            throw new InvalidOperationException("Пользователь не найден");

        var queryable = _dbContext.Views
            .Where(v => v.ApplicationUserId == userId);
        
        if (!string.IsNullOrEmpty(query))
            queryable = queryable.Where(v =>EF.Functions.Like(v.Video.Title, $"%{query}%"));

        queryable = queryable.OrderByDescending(v => v.UpdatedDate)
            .Skip(after)
            .Take(limit + 1);
        
        var videos = await queryable
            .Select(vv => vv.Video)
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