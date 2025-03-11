using AutoMapper;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class ViewService
    {
        private IUnitOfWorks _unitOfWorks;
        private VideoService _videoService;
        private IMapper _mapper;

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

            Video video = _unitOfWorks.Videos.Get(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            View view = _unitOfWorks.Views.Get(videoId, userId);

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

            View view = _unitOfWorks.Views.Get(videoId, userId);

            if (view == null)
                return;

            _unitOfWorks.Views.Remove(view);

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task ClearHistory(string userId)
        {
            ApplicationUser applicationUser = _unitOfWorks.ApplicationUsers.Get(userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            applicationUser.Views.Clear();

            await _unitOfWorks.SaveChangesAsync();
        }

        public List<ViewGetDTO> GetWithLimit(string userId, int limit, int after)
        {

            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            List<ViewGetDTO> result = new List<ViewGetDTO>();

            foreach (View view in _unitOfWorks.Views.GetHistoryWithLimit(userId, limit, after).ToList())
            {
                ViewGetDTO viewDTO = _mapper.Map<ViewGetDTO>(view);
                viewDTO.Video = _videoService.GetMinVideoById(view.VideoId, userId);
                result.Add(viewDTO);
            }

            return result;

        }
    }
}
