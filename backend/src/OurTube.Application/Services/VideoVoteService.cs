using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class VideoVoteService
    {
        private IUnitOfWorks _unitOfWorks;
        private PlaylistService _playlistService;

        public VideoVoteService(IUnitOfWorks unitOfWorks, PlaylistService playlistService)
        {
            _unitOfWorks = unitOfWorks;
            _playlistService = playlistService;
        }

        public async Task Set(int videoId, string userId, bool type)
        {
            Video video = _unitOfWorks.Videos.Get(videoId);
            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            VideoVote vote = _unitOfWorks.VideoVotes.Get(videoId, userId);

            if (vote == null)
            {
                _unitOfWorks.VideoVotes.Add(new VideoVote()
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


            Playlist playlist = _unitOfWorks.Playlists
                .Find(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId)
                .First();

            if (playlist == null)
            {
                await _playlistService.Create(new DTOs.Playlist.PlaylistPostDTO { Title = "Понравившееся" }, userId);
                playlist = _unitOfWorks.Playlists
                    .Find(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId)
                    .First();
            }

            if (type == true)
            {
                await _playlistService.AddVideo(playlist.Id, videoId, userId);
            }
            else
            {
                await _playlistService.RemoveVideo(playlist.Id, videoId, userId);
            }
        }

        public async Task Delete(int videoId, string userId)
        {
            Video video = _unitOfWorks.Videos.Get(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            VideoVote vote = _unitOfWorks.VideoVotes
                .Get(videoId, userId);

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

            Playlist playlist = _unitOfWorks.Playlists
                .Find(p => p.Title == "Понравившееся" && p.ApplicationUserId == userId).First();

            if (playlist == null)
            {
                await _playlistService.Create(new DTOs.Playlist.PlaylistPostDTO { Title = "Понравившееся" }, userId);
                return;
            }

            await _playlistService.RemoveVideo(playlist.Id, videoId, userId);

        }
    }
}
