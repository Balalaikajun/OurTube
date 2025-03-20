using AutoMapper;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class ViewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly VideoService _videoService;
        private readonly IMapper _mapper;

        public ViewService(IUnitOfWork unitOfWork, VideoService videoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task AddVideoAsync(int videoId, string userId, TimeSpan endTime)
        {
            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var video =await _unitOfWork.Videos.GetAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var view =await _unitOfWork.Views.GetAsync(videoId, userId);

            if (view != null)
            {
                view.DateTime = DateTime.UtcNow;
                view.EndTime = endTime;
            }
            else
            {
                _unitOfWork.Views.Add(new VideoView()
                {
                    ApplicationUserId = userId,
                    VideoId = videoId,
                    EndTime = endTime,
                    DateTime = DateTime.UtcNow
                });
                video.ViewsCount++;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveVideoAsync(int videoId, string userId)
        {
            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            if (!await _unitOfWork.Videos.ContainsAsync(videoId))
                throw new InvalidOperationException("Видео не найдено");

            var view =await _unitOfWork.Views.GetAsync(videoId, userId);

            if (view == null)
                return;

            _unitOfWork.Views.Remove(view);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ClearHistoryAsync(string userId)
        {
            var applicationUser =await _unitOfWork.ApplicationUsers.GetAsync(userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            var views = await _unitOfWork.Views.FindAsync(vv => vv.ApplicationUserId == applicationUser.Id);
            
            _unitOfWork.Views.RemoveRange(views);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ViewGetDto>> GetWithLimitAsync(string userId, int limit, int after)
        {

            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var result = new List<ViewGetDto>();

            foreach (var view in await _unitOfWork.Views.GetHistoryWithLimitAsync(userId, limit, after))
            {
                var viewDto = _mapper.Map<ViewGetDto>(view);
                viewDto.Video = await _videoService.GetMinVideoByIdAsync(view.VideoId, userId);
                result.Add(viewDto);
            }

            return result;

        }
    }
}
