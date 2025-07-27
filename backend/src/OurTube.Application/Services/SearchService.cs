using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;
using OurTube.Application.Requests.Video;

namespace OurTube.Application.Services;

public class SearchService : ISearchService
{
    private const int SearchPull = 25;
    private readonly IMemoryCache _cache;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IVideoService _videoService;

    public SearchService(IApplicationDbContext dbContext, IMemoryCache cache, IMapper mapper,
        IVideoService videoService)
    {
        _dbContext = dbContext;
        _cache = cache;
        _mapper = mapper;
        _videoService = videoService;
    }

    public async Task<ListReply<MinVideo>> SearchVideos(Guid userId, Guid sessionId, GetQueryParametersWithSearch parameters)
    {
        var cacheKey = GetCacheKey(sessionId);


        if (!_cache.TryGetValue(cacheKey, out List<Guid> cachePull))
        {
            cachePull = [];

            _cache.Set(cacheKey, cachePull, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }

        if (parameters.Reload)
            cachePull = [];

        if (cachePull.Count <= parameters.Limit +  parameters.After)
            cachePull.AddRange(await SearchMoreVideos(parameters.SearchQuery, sessionId, SearchPull));

        var videoIds = cachePull.Skip(parameters.After).Take(parameters.Limit).ToList();

        var videos = await _videoService.GetVideosByIdAsync(videoIds);

        return new ListReply<MinVideo>
        {
            HasMore = cachePull.Count > parameters.After + parameters.Limit,
            NextAfter = parameters.After + parameters.Limit,
            Elements = videos
        };
    }

    private async Task<IEnumerable<Guid>> SearchMoreVideos(string searchQuery, Guid sessionId,
        int limit = 10)
    {
        _cache.TryGetValue(GetCacheKey(sessionId), out List<Guid> viewedIds);

        var targetDate = DateTime.UtcNow.AddDays(-7);

        return await _dbContext.Videos
            .Where(v => EF.Functions.Like(v.Title, $"%{searchQuery}%"))
            .Where(v => !viewedIds.Contains(v.Id))
            .OrderBy(v => v.Views.Count(v => v.UpdatedDate >= targetDate))
            .Take(limit)
            .Select(x => x.Id)
            .ToListAsync();
    }

    private static string GetCacheKey(Guid sessionId)
    {
        return $"SearchService:{sessionId.ToString()}";
    }
}