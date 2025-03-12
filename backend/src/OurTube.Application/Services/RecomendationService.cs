using OurTube.Application.DTOs.Video;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class RecomendationService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly VideoService _videoService;

        public RecomendationService(IUnitOfWorks unitOfWorks, VideoService videoService)
        {
            _unitOfWorks = unitOfWorks;
            _videoService = videoService;
        }

        public List<VideoMinGetDto> GetVideos(int limit, int after, string? userId = null)
        {
            var videos = _unitOfWorks.Videos.GetAll()
                .OrderByDescending(v => v.LikesCount)
                .Skip(after)
                .Take(limit)
                .ToList();

            if (userId == null)
            {
                return videos
                    .Select(v => _videoService.GetMinVideoById(v.Id))
                    .ToList();
            }
            else
            {
                return videos
                    .Select(v => _videoService.GetMinVideoById(v.Id, userId))
                    .ToList();
            }
        }
    }
}
