using AutoMapper;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class ViewService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly VideoService _videoService;
        private readonly IMapper _mapper;

        public ViewService(IUnitOfWorks unitOfWorks, VideoService videoService, IMapper mapper)
        {
            _unitOfWorks = unitOfWorks;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task AddVideo(int videoId, string userId, long endTime)
        {
            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var video = _unitOfWorks.Videos.Get(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var view = _unitOfWorks.Views.Get(videoId, userId);

            if (view != null)
            {
                view.DateTime = DateTime.UtcNow;
                view.EndTime = endTime;
            }
            else
            {
                _unitOfWorks.Views.Add(new View()
                {
                    ApplicationUserId = userId,
                    VideoId = videoId,
                    EndTime = endTime
                });
                video.ViewsCount++;
            }

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task RemoveVideo(int videoId, string userId)
        {
            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            if (!_unitOfWorks.Videos.Contains(videoId))
                throw new InvalidOperationException("Видео не найдено");

            var view = _unitOfWorks.Views.Get(videoId, userId);

            if (view == null)
                return;

            _unitOfWorks.Views.Remove(view);

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task ClearHistory(string userId)
        {
            var applicationUser = _unitOfWorks.ApplicationUsers.Get(userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            applicationUser.Views.Clear();

            await _unitOfWorks.SaveChangesAsync();
        }

        public List<ViewGetDto> GetWithLimit(string userId, int limit, int after)
        {

            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var result = new List<ViewGetDto>();

            foreach (var view in _unitOfWorks.Views.GetHistoryWithLimit(userId, limit, after).ToList())
            {
                var viewDto = _mapper.Map<ViewGetDto>(view);
                viewDto.Video = _videoService.GetMinVideoById(view.VideoId, userId);
                result.Add(viewDto);
            }

            return result;

        }
    }
}
