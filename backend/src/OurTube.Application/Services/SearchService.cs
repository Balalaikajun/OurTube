using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NpgsqlTypes;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;

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

    public async Task<ListReply<MinVideo>> SearchVideos(Guid? userId, Guid sessionId,
        GetQueryParametersWithSearch parameters)
    {
        var cacheKey = GetCacheKey(sessionId);

        if (!_cache.TryGetValue(cacheKey, out (string Query, List<Guid> Pull) meta))
        {
            meta = (string.Empty, new List<Guid>());
            _cache.Set(cacheKey, meta, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }

        var normalizedQuery = (parameters.Query ?? string.Empty).Trim().ToLowerInvariant();

        if (meta.Query != normalizedQuery)
        {
            meta.Pull.Clear();
            meta.Query = normalizedQuery;
            _cache.Set(cacheKey, meta, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }

        var cachePull = meta.Pull;

        var neededCount = parameters.After + parameters.Limit - cachePull.Count;
        if (neededCount > 0)
        {
            var more = (await SearchMoreVideos(parameters.Query, cachePull, neededCount)).ToList();
            cachePull.AddRange(more.Where(id => !cachePull.Contains(id)));

            _cache.Set(cacheKey, meta, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            });
        }

        var videoIds = cachePull.Skip(parameters.After).Take(parameters.Limit).ToList();

        var videos = await _videoService.GetVideosByIdAsync(videoIds) ?? new List<MinVideo>();

        var videosOrdered = videoIds
            .Select(id => videos.FirstOrDefault(v => v.Id == id))
            .Where(v => v != null)
            .ToList();

        return new ListReply<MinVideo>
        {
            HasMore = cachePull.Count > parameters.After + parameters.Limit,
            NextAfter = parameters.After + parameters.Limit,
            Elements = videosOrdered
        };
    }
    
    private async Task<IEnumerable<Guid>> SearchMoreVideos(
        string searchQuery,
        IEnumerable<Guid> excludeIds,
        int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return Enumerable.Empty<Guid>();

        var targetDate = DateTime.UtcNow.AddDays(-7);

        var query = _dbContext.Videos
            .Where(v => EF.Property<NpgsqlTsVector>(v, "SearchVector")
                .Matches(EF.Functions.PlainToTsQuery("simple", searchQuery)));

        if (excludeIds.Any())
            query = query.Where(v => !excludeIds.Contains(v.Id));

        query = query.OrderByDescending(v => v.Views.Count(view => view.UpdatedDate >= targetDate));

        return await query
            .Take(limit)
            .Select(x => x.Id)
            .ToListAsync();
    }

    private static string GetCacheKey(Guid sessionId)
    {
        return $"SearchService:{sessionId.ToString()}";
    }
}