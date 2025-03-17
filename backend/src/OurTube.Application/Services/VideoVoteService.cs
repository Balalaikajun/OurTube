using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class VideoVoteService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly PlaylistService _playlistService;

        public VideoVoteService(IUnitOfWorks unitOfWorks, PlaylistService playlistService)
        {
            _unitOfWorks = unitOfWorks;
            _playlistService = playlistService;
        }

        public async Task SetAsync(int videoId, string userId, bool type)
        {
            var video =await _unitOfWorks.Videos.GetAsync(videoId);
            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWorks.VideoVotes.GetAsync(videoId, userId);

            if (vote == null)
            {
                _unitOfWorks.VideoVotes.Add(new VideoVote
                {
                    ApplicationUserId = userId,
                    VideoId = videoId,
                    Type = type
                });

                if (type == true)
                {
                    video.LikesCount++;
                }
                else
                {
                    video.DislikeCount++;
                }
            }
            else if (vote.Type != type)
            {
                vote.Type = type;

                if (type == true)
                {
                    video.DislikeCount--;
                    video.LikesCount++;
                }
                else
                {
                    video.DislikeCount++;
                    video.LikesCount--;

                }
            }
            else
            {
                return;
            }


            var playlist =(await _unitOfWorks.Playlists
                .FindAsync(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId))
                .First();

            if (playlist == null)
            {
                await _playlistService.CreateAsync(new DTOs.Playlist.PlaylistPostDto { Title = "Понравившееся" }, userId);
                playlist =(await _unitOfWorks.Playlists
                    .FindAsync(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId))
                    .First();
            }

            if (type == true)
            {
                await _playlistService.AddVideoAsync(playlist.Id, videoId, userId);
            }
            else
            {
                await _playlistService.RemoveVideoAsync(playlist.Id, videoId, userId);
            }
        }

        public async Task DeleteAsync(int videoId, string userId)
        {
            var video =await _unitOfWorks.Videos.GetAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWorks.VideoVotes
                .GetAsync(videoId, userId);

            if (vote == null)
                return;

            if (vote.Type == true)
            {
                video.LikesCount--;
            }
            else
            {
                video.DislikeCount--;
            }

            _unitOfWorks.VideoVotes.Remove(vote);

            var playlist =(await _unitOfWorks.Playlists
                .FindAsync(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId))
                .First();

            if (playlist == null)
            {
                await _playlistService.CreateAsync(new DTOs.Playlist.PlaylistPostDto { Title = "Понравившееся" }, userId);
                return;
            }

            await _playlistService.RemoveVideoAsync(playlist.Id, videoId, userId);

        }
    }
}
