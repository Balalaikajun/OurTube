using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;

namespace OurTube.Application.Services
{
    public class VoteService
    {
        private ApplicationDbContext _dbContext;
        private PlaylistService _playlistService;

        public VoteService(ApplicationDbContext dbContext, PlaylistService playlistService)
        {
            _dbContext = dbContext;
            _playlistService = playlistService;
        }

        public async Task Set(int videoId, string userId, bool type)
        {
            Video video = _dbContext.Videos.FirstOrDefault(v => v.Id == videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(u => u.Votes)
                .Include(u => u.Playlists)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            Vote vote = applicationUser.Votes
                .FirstOrDefault(v => v.VideoId == videoId);

            if (vote == null)
            {
                applicationUser.Votes.Add(new Vote()
                {
                    ApplicationUserId = userId,
                    VideoId = videoId,
                    Type = type
                });
            }
            else if (vote.Type != type)
            {
                vote.Type = type;
            }
            else
            {
                return;
            }


            Playlist playlist = applicationUser.Playlists
                .FirstOrDefault(p => p.Title == "Понравившееся");
            if (playlist == null)
            {
                await _playlistService.Create(new DTOs.Playlist.PlaylistPostDTO { Title = "Понравившееся" }, userId);
                playlist = applicationUser.Playlists
                .FirstOrDefault(p => p.Title == "Понравившееся");
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
            Video video = _dbContext.Videos.FirstOrDefault(v => v.Id == videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(u => u.Playlists)
                .Include(u => u.Votes)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            Vote vote = applicationUser.Votes
                .FirstOrDefault(v => v.VideoId == videoId);

            if (vote == null)
                return;

            applicationUser.Votes.Remove(vote);

            await _dbContext.SaveChangesAsync();

            Playlist playlist = applicationUser.Playlists
                .FirstOrDefault(p => p.Title == "Понравившееся");
            if (playlist == null)
                await _playlistService.Create(new DTOs.Playlist.PlaylistPostDTO { Title = "Понравившееся" }, userId);

            await _playlistService.RemoveVideo(playlist.Id, videoId, userId);

        }
    }
}
