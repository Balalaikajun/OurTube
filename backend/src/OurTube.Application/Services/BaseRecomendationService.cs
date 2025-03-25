using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class BaseRecomendationService:IRecomendationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VideoService _videoService;

        public BaseRecomendationService(IUnitOfWork unitOfWork, VideoService videoService)
        {
            _unitOfWork = unitOfWork;
            _videoService = videoService;
        }

        public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after, string? userId = null)
        {
            var videos =await _unitOfWork.Videos.GetAll()
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
            var videos =await _unitOfWork.Videos.GetAll()
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
