using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Interfaces;
using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Services;

public class AdvancedRecomendationService:IRecomendationService
{
    private readonly IRecomendationRepository _recomendationRepository;
    private readonly VideoService _videoService;

    public AdvancedRecomendationService(IRecomendationRepository recomendationRepository, VideoService videoService)
    {
        _recomendationRepository = recomendationRepository;
        _videoService = videoService;
    }
    
    public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after)
    {
        var popular = await _recomendationRepository.GetIndexesPopularAsync();

        var tasks = popular
            .Select(async v => await _videoService.GetMinVideoByIdAsync(v));

        var result = await Task.WhenAll(tasks);
        
        // Перемешиваем
        Random.Shared.Shuffle(result);
        
        return result.Skip(after).Take(limit);
    }

    public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after, string userId)
    {
        var popular = await _recomendationRepository.GetIndexesPopularAsync();
        var byTags = await _recomendationRepository.GetIndexesByTagsAsync( userId);
        var bySubscribition = await _recomendationRepository.GetIndexesBySubscriptionAsync(userId);

        var videosIds = popular
            .Concat(byTags)
            .Concat(bySubscribition)
            .Distinct();

        var result = await Task.WhenAll(
            videosIds.Select(async v => await _videoService.GetMinVideoByIdAsync(v, userId)));
        
        // Перемешиваем
        Random.Shared.Shuffle(result);
        
        return result.Skip(after).Take(limit);
    }
    
    
}