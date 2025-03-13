using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class BaseRecomendationService:IRecomendationService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly VideoService _videoService;

        public BaseRecomendationService(IUnitOfWorks unitOfWorks, VideoService videoService)
        {
            _unitOfWorks = unitOfWorks;
            _videoService = videoService;
        }

        public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after, string? userId = null)
        {
            var videos = _unitOfWorks.Videos.GetAll()
                .OrderByDescending(v => v.LikesCount)
                .Skip(after)
                .Take(limit);

            if (userId == null)
            {
                return await videos
                    .Select(v => _videoService.GetMinVideoById(v.Id))
                    .ToListAsync();
            }
            else
            {
                return await videos
                    .Select(v => _videoService.GetMinVideoById(v.Id, userId))
                    .ToListAsync();
            }
        }
    }
}
