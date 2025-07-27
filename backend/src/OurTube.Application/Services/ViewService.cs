using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Replies.Views;
using OurTube.Application.Requests.Common;
using OurTube.Application.Requests.Views;
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

    public async Task AddVideoAsync(Guid userId, Guid videoId, PostViewsRequest request)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);

        var video = await _dbContext.Videos
            .GetByIdAsync(videoId, true);

        var view = await _dbContext.Views.FindAsync(videoId, userId);

        if (view != null)
        {
            view.EndTime = request.EndTime;
        }
        else
        {
            view = new VideoView
            {
                ApplicationUserId = userId,
                VideoId = videoId,
                EndTime = request.EndTime,
            };

            _dbContext.Views.Add(view);
            video.ViewsCount++;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveVideoAsync(Guid videoId, Guid userId)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);

        await _dbContext.Videos
            .EnsureExistAsync(videoId);

        var view = await _dbContext.Views
            .GetAsync(vv => vv.VideoId==videoId && vv.ApplicationUserId == userId, true);

        view.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearHistoryAsync(Guid userId)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);

        var views = await _dbContext.Views
            .Where(vv => vv.ApplicationUserId == userId)
            .ToListAsync();

        foreach (var view in views)
        {
            view.Delete();
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ListReply<MinVideo>> GetWithLimitAsync(Guid userId, GetQueryParametersWithSearch parameter)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);

        var queryable = _dbContext.Views
            .Where(v => v.ApplicationUserId == userId);
        
        if (!string.IsNullOrEmpty(parameter.SearchQuery))
            queryable = queryable.Where(v =>EF.Functions.Like(v.Video.Title, $"%{parameter.SearchQuery}%"));

        queryable = queryable.OrderByDescending(v => v.UpdatedDate)
            .Skip(parameter.After)
            .Take(parameter.Limit + 1);
        
        var videos = await queryable
            .Select(vv => vv.Video)
            .ProjectToMinDto(_mapper, userId)
            .ToListAsync();

        return new ListReply<MinVideo>()
        {
            Elements = videos.Take(parameter.Limit),
            NextAfter = parameter.After + parameter.Limit,
            HasMore = videos.Count > parameter.Limit
        };
    }
}