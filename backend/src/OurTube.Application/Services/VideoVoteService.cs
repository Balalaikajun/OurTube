using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class VideoVoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PlaylistService _playlistService;

        public VideoVoteService(IUnitOfWork unitOfWork, PlaylistService playlistService)
        {
            _unitOfWork = unitOfWork;
            _playlistService = playlistService;
        }

        public async Task SetAsync(int videoId, string userId, bool type)
        {
            var video =await _unitOfWork.Videos.GetAsync(videoId);
            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWork.VideoVotes.GetAsync(videoId, userId);

            if (vote == null)
            {
                _unitOfWork.VideoVotes.Add(new VideoVote(videoId, userId, type));
            }
            else if(vote.Type != type)
            {
                vote.Update(type);    
            }
            else
            {
                return;
            }
            
            
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int videoId, string userId)
        {
            var video =await _unitOfWork.Videos.GetAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWork.VideoVotes
                .GetAsync(videoId, userId);

            if (vote == null)
                return;
 
            vote.RemoveEvent();

            _unitOfWork.VideoVotes.Remove(vote);

            await _unitOfWork.SaveChangesAsync();

        }
    }
}
