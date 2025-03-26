using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories;

public class RecomendationRepository: IRecomendationRepository
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly DateTime _dateTrashold;
    private readonly double _popularityWeight = 0.001;
    private readonly double _randomWeight = 0.1;

    public RecomendationRepository(ApplicationDbContext context)
    {
        _applicationDbContext = context;
        _dateTrashold = DateTime.Now.AddDays(-14);
    }
    
    public async Task<IEnumerable<int>> GetIndexesPopularAsync()
    {
        var result = await _applicationDbContext.Videos
            .Select(v => new
            {
                Video = v,
                Score = v.Views
                    .Count(vi => vi.DateTime > _dateTrashold)
            })
            .OrderByDescending(v => v.Score)
            .Take(10)
            .Select(v => v.Video.Id)
            .ToListAsync();

        return result;
    }
    
    public async Task<IEnumerable<int>> GetIndexesByTagsAsync(string userId)
    {
        var views = await _applicationDbContext.Views
            .Where(vi => vi.DateTime > _dateTrashold)
            .Select(vi => vi.VideoId)
            .ToHashSetAsync();
        
        var userTags = _applicationDbContext.VideoTags
            .Where(vt => views.Contains(vt.VideoId))
            .GroupBy(t => t.TagId)
            .OrderByDescending(g => g.Count())
            .ToDictionary(v => v.Key, v => v.Count());

        var result =await _applicationDbContext.Videos
            .Select(v => new
            {
                Video = v,
                Score = v.Tags.Sum(t => userTags.GetValueOrDefault(t.TagId, 0)) +
                        EF.Functions.Random() * _popularityWeight,
            })
            .Where(v => !views.Contains(v.Video.Id))
            .OrderByDescending(v => v.Score)
            .Select(v => v.Video.Id)
            .Take(10)
            .ToListAsync();

        return result;
    }

    public async Task<IEnumerable<int>> GetIndexesBySubscriptionAsync(string userId)
    {
        var subscriptions = await _applicationDbContext.Subscriptions
            .Where(s => s.SubscriberId == userId)
            .Select(s => s.SubscribedToId)
            .ToListAsync();

        var result = await _applicationDbContext.Videos
            .Where(v => v.Created > _dateTrashold && subscriptions.Contains(v.ApplicationUserId))
            .Select(v => v.Id)
            .ToListAsync();

        return result;
    }
}