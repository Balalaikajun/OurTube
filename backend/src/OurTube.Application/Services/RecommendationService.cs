using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Recommendation;

namespace OurTube.Application.Services;

public class RecommendationService : IRecomendationService
{
    private const int RecommendationPullCount = 20;
    private const string VideoCountCacheKey = "TotalVideoCount";

    private const double WeeksViewsRate = 0.7;
    private const double WeeksLikesRate = 0.3;
    private readonly IMemoryCache _cache;
    private readonly IApplicationDbContext _dbContext;
    private readonly IVideoService _videoService;

    public RecommendationService(IApplicationDbContext dbContext, IMemoryCache cache, IVideoService videoService)
    {
        _dbContext = dbContext;
        _cache = cache;
        _videoService = videoService;
    }

    public async Task<ListReply<MinVideo>> GetRecommendationsAsync(GetRecommendationsRequest request)
    {
        var cacheKey = GetRecommendationsCacheKey(request.SessionId);

        if (request.Reload)
            _cache.Remove(cacheKey);

        if (!_cache.TryGetValue(cacheKey, out List<Guid> cachedRecommendations))
        {
            cachedRecommendations = new List<Guid>();

            _cache.Set(cacheKey, cachedRecommendations, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        if (!_cache.TryGetValue(VideoCountCacheKey, out int totalVideoCount))
        {
            totalVideoCount = await _dbContext.Videos.CountAsync();

            _cache.Set(VideoCountCacheKey, totalVideoCount, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1)
            });
        }

        if (cachedRecommendations.Count <= request.After + request.Limit && cachedRecommendations.Count < totalVideoCount)
        {
            if (!string.IsNullOrEmpty(request.UserId.ToString()))
                cachedRecommendations.AddRange(
                    await LoadAuthorizedRecommendationsAsync(request.UserId!.Value, request.SessionId, RecommendationPullCount));
            else
                cachedRecommendations.AddRange(
                    await LoadAnAuthorizedRecommendationsAsync(request.SessionId, RecommendationPullCount));
        }

        var resultIds = cachedRecommendations.Skip(request.After).Take(request.Limit).ToList();

        var result = await _videoService.GetVideosByIdAsync(resultIds);

        return new ListReply<MinVideo>()
        {
            Elements= result,
            NextAfter = request.After + request.Limit,
            HasMore = cachedRecommendations.Count > request.After + request.Limit
        };
    }

    public async Task<ListReply<MinVideo>> GetRecommendationsForVideoAsync(GetRecommendationsForVideoRequest request)
    {
        var result = await GetRecommendationsAsync(new GetRecommendationsRequest
        {
           UserId = request.UserId,
            SessionId = request.SessionId,
            Limit = request.Limit,
            After = request.After,
            Reload = request.Reload
        });

        if (result.Elements.Any(v => v.Id == request.VideoId))
        {
            var oneMore = await GetRecommendationsAsync(new GetRecommendationsRequest
            {
                UserId = request.UserId,
                SessionId = request.SessionId,
                Limit = 1,
                After = request.After,
                Reload = false
            });

            var videos = result.Elements.ToList();
            videos.Remove(videos.First(v => v.Id == request.VideoId));
            var newVideo = oneMore.Elements.FirstOrDefault();
            if(newVideo != null)
                videos.Add(newVideo);
            result.Elements = videos;
            result.NextAfter = oneMore.NextAfter;
        }
        
        return result;
    }

    private async Task<IEnumerable<Guid>> LoadAuthorizedRecommendationsAsync(Guid userId, Guid sessionId, int limit)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);
        
        const double trendRecRatio = 0.4;
        const double popRecRatio = 0.2;
        const double tagsRecRatio = 0.2;
        const double subsRecRatio = 0.2;

        var trendRec = (await GetTrendsRecommendationsAsync(sessionId, limit)).ToList();
        var popRec = (await GetPopularityRecommendationsAsync(sessionId, limit)).ToList();
        var tagsRec = (await GetTagsRecommendationsAsync(userId, sessionId, limit)).ToList();
        var subsRec = (await GetSubscriptionRecommendationsAsync(userId, sessionId, limit)).ToList();

        var result = new List<Guid>(limit);
        var used = new HashSet<Guid>();

        var trendIndex = 0;
        var popIndex = 0;
        var tagIndex = 0;
        var subsIndex = 0;

        var trendCount = (int)Math.Round(limit * trendRecRatio);
        var popCount = (int)Math.Round(limit * popRecRatio);
        var tagCount = (int)Math.Round(limit * tagsRecRatio);
        var subsCount = (int)Math.Round(limit * subsRecRatio);

        while (result.Count < limit &&
               (trendIndex < trendRec.Count || popIndex < popRec.Count || tagIndex < tagsRec.Count ||
                subsIndex < subsRec.Count))
        {
            for (var i = 0; i < trendCount && trendIndex < trendRec.Count; i++)
            {
                var id = trendRec[trendIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }

            if (result.Count >= limit) break;

            for (var i = 0; i < popCount && popIndex < popRec.Count; i++)
            {
                var id = popRec[popIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }

            if (result.Count >= limit) break;

            for (var i = 0; i < tagCount && tagIndex < tagsRec.Count; i++)
            {
                var id = tagsRec[tagIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }

            if (result.Count >= limit) break;

            for (var i = 0; i < subsCount && subsIndex < subsRec.Count; i++)
            {
                var id = subsRec[subsIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }
        }

        return result;
    }

    private async Task<IEnumerable<Guid>> LoadAnAuthorizedRecommendationsAsync(Guid sessionId,
        int limit)
    {
        const double trendRecRatio = 0.6;
        const double popRecRatio = 0.4;

        var trendRec = (await GetTrendsRecommendationsAsync(sessionId, limit)).ToList();
        var popRec = (await GetPopularityRecommendationsAsync(sessionId, limit)).ToList();

        var result = new List<Guid>(limit);
        var used = new HashSet<Guid>();

        var trendIndex = 0;
        var popIndex = 0;

        var trendCount = (int)Math.Round(limit * trendRecRatio);
        var popCount = (int)Math.Round(limit * popRecRatio);

        while (result.Count < limit &&
               (trendIndex < trendRec.Count || popIndex < popRec.Count))
        {
            for (var i = 0; i < trendCount && trendIndex < trendRec.Count; i++)
            {
                var id = trendRec[trendIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }

            if (result.Count >= limit) break;

            for (var i = 0; i < popCount && popIndex < popRec.Count; i++)
            {
                var id = popRec[popIndex++];
                if (used.Add(id))
                {
                    result.Add(id);
                    if (result.Count >= limit) break;
                }
            }
        }

        return result;
    }

    private async Task<IEnumerable<Guid>> GetTrendsRecommendationsAsync(Guid sessionId, int limit)
    {
        _cache.TryGetValue(GetRecommendationsCacheKey(sessionId), out List<Guid>? usedIds);
        usedIds ??= [];

        var targetDate = DateTime.UtcNow.AddDays(-7);

        var candidates = await _dbContext.Videos
            .Where(v => v.CreatedDate >= targetDate)
            .Where(v => !usedIds.Contains(v.Id))
            .Select(v => new
            {
                v.Id,
                ViewsCount = v.Views.Count(vi => vi.UpdatedDate >= targetDate),
                LikesCount = v.Votes.Count(vi => vi.UpdatedDate >= targetDate && vi.Type == true)
            })
            .ToListAsync();

        if (candidates.Count == 0)
            return [];

        var minV = candidates.Min(x => x.ViewsCount);
        var maxV = candidates.Max(x => x.ViewsCount);
        var minL = candidates.Min(x => x.LikesCount);
        var maxL = candidates.Max(x => x.LikesCount);

        if (maxV == minV) maxV = minV + 1;
        if (maxL == minL) maxL = minL + 1;

        var result = candidates
            .Select(c =>
            {
                var normV = (c.ViewsCount - minV) / (double)(maxV - minV);
                var normL = (c.LikesCount - minL) / (double)(maxL - minL);
                var score = WeeksViewsRate * normV + WeeksLikesRate * normL;

                return new { c.Id, Score = score };
            })
            .OrderByDescending(r => r.Score)
            .Take(limit)
            .Select(r => r.Id)
            .ToList();

        return result;
    }

    private async Task<IEnumerable<Guid>> GetPopularityRecommendationsAsync(Guid sessionId, int limit)
    {
        _cache.TryGetValue(GetRecommendationsCacheKey(sessionId), out List<Guid>? usedIds);
        usedIds ??= [];

        var sortTargetDate = DateTime.UtcNow.AddDays(-7);

        var candidates = await _dbContext.Videos
            .Where(v => !usedIds.Contains(v.Id))
            .Select(v => new
            {
                v.Id,
                ViewsCount = v.Views.Count(vi => vi.UpdatedDate >= sortTargetDate),
                LikesCount = v.Votes.Count(vi => vi.UpdatedDate >= sortTargetDate && vi.Type == true)
            })
            .ToListAsync();

        if (candidates.Count == 0)
            return [];

        var minV = candidates.Min(x => x.ViewsCount);
        var maxV = candidates.Max(x => x.ViewsCount);
        var minL = candidates.Min(x => x.LikesCount);
        var maxL = candidates.Max(x => x.LikesCount);

        if (maxV == minV) maxV = minV + 1;
        if (maxL == minL) maxL = minL + 1;

        var result = candidates
            .Select(c =>
            {
                var normV = (c.ViewsCount - minV) / (double)(maxV - minV);
                var normL = (c.LikesCount - minL) / (double)(maxL - minL);
                var score = WeeksViewsRate * normV + WeeksLikesRate * normL;

                return new { c.Id, Score = score };
            })
            .OrderByDescending(r => r.Score)
            .Take(limit)
            .Select(r => r.Id)
            .ToList();

        return result;
    }

    private async Task<IEnumerable<Guid>> GetTagsRecommendationsAsync(Guid userId, Guid sessionId, int limit)
    {
        _cache.TryGetValue(GetRecommendationsCacheKey(sessionId), out List<Guid>? usedIds);
        usedIds ??= [];

        var targetDate = DateTime.UtcNow.AddDays(-7);

        var userTags = await (
            from vi in _dbContext.Views
            join vt in _dbContext.VideoTags on vi.VideoId equals vt.VideoId
            where vi.ApplicationUserId == userId && vi.UpdatedDate >= targetDate
            select vt.TagId
        ).Distinct().ToListAsync();

        if (userTags.Count == 0)
            return [];

        var candidates = await (
            from v in _dbContext.Videos
            where !usedIds.Contains(v.Id)
            let tagMatchCount = v.Tags.Count(t => userTags.Contains(t.TagId))
            where tagMatchCount > 0
            select new
            {
                v.Id,
                TagMatch = tagMatchCount,
                ViewsCount = v.Views.Count(vi => vi.UpdatedDate >= targetDate),
                LikesCount = v.Votes.Count(vi => vi.UpdatedDate >= targetDate && vi.Type == true)
            }
        ).ToListAsync();

        if (candidates.Count == 0)
            return [];

        var minV = candidates.Min(x => x.ViewsCount);
        var maxV = candidates.Max(x => x.ViewsCount);
        var minL = candidates.Min(x => x.LikesCount);
        var maxL = candidates.Max(x => x.LikesCount);

        if (maxV == minV) maxV = minV + 1;
        if (maxL == minL) maxL = minL + 1;

        var result = candidates
            .Select(c =>
            {
                var normV = (c.ViewsCount - minV) / (double)(maxV - minV);
                var normL = (c.LikesCount - minL) / (double)(maxL - minL);
                var score = WeeksViewsRate * normV + WeeksLikesRate * normL;

                return new { c.Id, c.TagMatch, Score = score };
            })
            .OrderByDescending(r => r.TagMatch)
            .ThenByDescending(r => r.Score)
            .Take(limit)
            .Select(c => c.Id)
            .ToList();

        return result;
    }

    private async Task<IEnumerable<Guid>> GetSubscriptionRecommendationsAsync(Guid userId, Guid sessionId, int limit)
    {
        _cache.TryGetValue(GetRecommendationsCacheKey(sessionId), out List<Guid>? usedIds);
        usedIds ??= [];

        var targetDate = DateTime.UtcNow.AddDays(-7);

        var subs = await _dbContext.Subscriptions
            .Where(s => s.SubscriberId == userId)
            .Select(s => s.SubscribedToId)
            .ToListAsync();

        if (subs.Count == 0)
            return new List<Guid>();

        var candidates = await _dbContext.Videos
            .Where(v => !usedIds.Contains(v.Id) && subs.Contains(v.ApplicationUserId))
            .Select(v => new
            {
                v.Id,
                ViewsCount = v.Views.Count(vi => vi.UpdatedDate >= targetDate),
                LikesCount = v.Votes.Count(vi => vi.UpdatedDate >= targetDate && vi.Type == true)
            })
            .ToListAsync();

        if (candidates.Count == 0)
            return [];

        var minV = candidates.Min(x => x.ViewsCount);
        var maxV = candidates.Max(x => x.ViewsCount);
        var minL = candidates.Min(x => x.LikesCount);
        var maxL = candidates.Max(x => x.LikesCount);

        if (maxV == minV) maxV = minV + 1;
        if (maxL == minL) maxL = minL + 1;

        var result = candidates
            .Select(c =>
            {
                var normV = (c.ViewsCount - minV) / (double)(maxV - minV);
                var normL = (c.LikesCount - minL) / (double)(maxL - minL);
                var score = WeeksViewsRate * normV + WeeksLikesRate * normL;

                return new { c.Id, Score = score };
            })
            .OrderByDescending(r => r.Score)
            .Take(limit)
            .Select(c => c.Id)
            .ToList();

        return result;
    }

    private static string GetRecommendationsCacheKey(Guid sessionId)
    {
        return $"Recommendations_{sessionId.ToString()}";
    }
}