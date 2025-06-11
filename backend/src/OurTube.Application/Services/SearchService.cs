using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;

namespace OurTube.Application.Services;

public class SearchService
{
    private const int SearchPull = 25;
    private readonly IMemoryCache _cache;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly VideoService _videoService;

    public SearchService(IApplicationDbContext dbContext, IMemoryCache cache, IMapper mapper, VideoService videoService)
    {
        _dbContext = dbContext;
        _cache = cache;
        _mapper = mapper;
        _videoService = videoService;
    }

    public async Task<IEnumerable<VideoMinGetDto>> SearchVideos(string searchQuery, string? userId, string sessionId,
        int limit = 10,
        int after = 0, bool reload = true)
    {
        var cacheKey = GetCacheKey(sessionId);


        if (!_cache.TryGetValue(cacheKey, out List<int> cachePull))
        {
            cachePull = [];

            _cache.Set(cacheKey, cachePull, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }

        if (reload)
            cachePull = [];

        if (cachePull.Count < limit + after)
            cachePull.AddRange(await SearchMoreVideos(searchQuery, sessionId, SearchPull));

        var resultIds = cachePull.Skip(after).Take(limit).ToList();

        var result = await _videoService.GetVideosByIdAsync(resultIds);

        return result;
    }

    private async Task<IEnumerable<int>> SearchMoreVideos(string searchQuery, string sessionId,
        int limit = 10)
    {
        _cache.TryGetValue(GetCacheKey(sessionId), out List<int> viewedIds);

        var targetDate = DateTime.UtcNow.AddDays(-7);

        return await _dbContext.Videos
            .Where(v => EF.Functions.Like(v.Title, $"%{searchQuery}%"))
            .Where(v => !viewedIds.Contains(v.Id))
            .OrderBy(v => v.Views.Count(v => v.DateTime >= targetDate))
            .Take(limit)
            .Select(x => x.Id)
            .ToListAsync();
    }

    private static string GetCacheKey(string sessionId)
    {
        return $"SearchService:{sessionId}";
    }
}