using OurTube.Application.DTOs.Video;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class RecomendationService
    {
        private ApplicationDbContext _dbContext;
        private VideoService _videoService;

        public RecomendationService(ApplicationDbContext dbContext, VideoService videoService)
        {
            _dbContext = dbContext;
            _videoService = videoService;
        }

        public List<VideoMinGetDTO> GetVideos(int limit, int after, string? userId = null)
        {
            var videos = _dbContext.Videos
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
