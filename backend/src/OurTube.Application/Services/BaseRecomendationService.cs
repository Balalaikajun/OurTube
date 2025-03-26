using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
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

        private async Task<IEnumerable<int>> GetVideosAsync(int limit, int after)
        {
            var videos =await _unitOfWork.Videos.GetAll()
                .OrderByDescending(v => v.LikesCount)
                .Skip(after)
                .Take(limit)
                .Select(v => v.Id)
                .ToListAsync();

            return videos;
        }

        public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after, string userId)
        {
            var videos =await GetVideosAsync(limit, after);
            
            var result = await Task.WhenAll( 
                videos.Select(async v => await _videoService.GetMinVideoByIdAsync(v, userId)));
            
            return result;
        }
        
        public async Task<IEnumerable<VideoMinGetDto>> GetRecomendationsAsync(int limit, int after)
        {
            var videos =await GetVideosAsync(limit, after);
            
            var result = await Task.WhenAll( 
                videos.Select(async v => await _videoService.GetMinVideoByIdAsync(v)));
            
            return result;
        }
        
    }
}
