using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class BaseRecomendationService:IRecomendationService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly VideoService _videoService;

        public BaseRecomendationService(IApplicationDbContext dbContext, VideoService videoService)
        {
            _dbContext = dbContext;
            _videoService = videoService;
        }

        public async Task<IEnumerable<VideoMinGetDto>> GetRecommendationsAsync(string? userId, string sessionId,
            int limit, int after,
            bool reload = false)
        {
            var videos =await _dbContext.Videos
                .OrderByDescending(v => v.LikesCount)
                .Skip(after)
                .Take(limit)
                .Select(v => v.Id)
                .ToListAsync();

            var result = new List<VideoMinGetDto>();
            if (userId == null)
            {
                foreach (var videoId in videos)
                {
                    result.Add(await _videoService.GetMinVideoByIdAsync(videoId));
                }
            }
            else
            {
                foreach (var videoId in videos)
                {
                    result.Add(await _videoService.GetMinVideoByIdAsync(videoId, userId));
                }
            }

            return result;
        }
        
        public async Task<IEnumerable<VideoMinGetDto>> GetAdvanceRecomendationsAsync(int limit, int after, string? userId = null)
        {
            var videos =await _dbContext.Videos
                .OrderByDescending(v => v.LikesCount)
                .Skip(after)
                .Take(limit)
                .Select(v => v.Id)
                .ToListAsync();

            var result = new List<VideoMinGetDto>();
            if (userId == null)
            {
                foreach (var videoId in videos)
                {
                    result.Add(await _videoService.GetMinVideoByIdAsync(videoId));
                }
            }
            else
            {
                foreach (var videoId in videos)
                {
                    result.Add(await _videoService.GetMinVideoByIdAsync(videoId, userId));
                }
            }

            return result;
        }
    }
}
