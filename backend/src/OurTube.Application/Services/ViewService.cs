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

        public async Task AddVideoAsync(int videoId, string userId, TimeSpan endTime)
        {
            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var video =await _unitOfWorks.Videos.GetAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var view =await _unitOfWorks.Views.GetAsync(videoId, userId);

            if (view != null)
            {
                view.DateTime = DateTime.UtcNow;
                view.EndTime = endTime;
            }
            else
            {
                _unitOfWorks.Views.Add(new VideoView()
                {
                    ApplicationUserId = userId,
                    VideoId = videoId,
                    EndTime = endTime,
                    DateTime = DateTime.UtcNow
                });
                video.ViewsCount++;
            }

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task RemoveVideoAsync(int videoId, string userId)
        {
            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            if (!await _unitOfWorks.Videos.ContainsAsync(videoId))
                throw new InvalidOperationException("Видео не найдено");

            var view =await _unitOfWorks.Views.GetAsync(videoId, userId);

            if (view == null)
                return;

            _unitOfWorks.Views.Remove(view);

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task ClearHistoryAsync(string userId)
        {
            var applicationUser =await _unitOfWorks.ApplicationUsers.GetAsync(userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            var views = await _unitOfWorks.Views.FindAsync(vv => vv.ApplicationUserId == applicationUser.Id);
            
            _unitOfWorks.Views.RemoveRange(views);

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task<List<ViewGetDto>> GetWithLimitAsync(string userId, int limit, int after)
        {

            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var result = new List<ViewGetDto>();

            foreach (var view in await _unitOfWorks.Views.GetHistoryWithLimitAsync(userId, limit, after))
            {
                var viewDto = _mapper.Map<ViewGetDto>(view);
                viewDto.Video = await _videoService.GetMinVideoByIdAsync(view.VideoId, userId);
                result.Add(viewDto);
            }

            return result;

        }
    }
}
