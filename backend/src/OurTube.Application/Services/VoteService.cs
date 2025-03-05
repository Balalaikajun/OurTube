using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class VoteService
    {
        private ApplicationDbContext _dbContext;
        
        public VoteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Set(int videoId, string userId, bool type)
        {
            if (_dbContext.Videos.FirstOrDefault(v => v.Id== videoId) == null)
                throw new InvalidOperationException("Видео не найдено");

            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(u => u.Votes)
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

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int videoId, string userId)
        {
            if (_dbContext.Videos.FirstOrDefault(v => v.Id == videoId) == null)
                throw new InvalidOperationException("Видео не найдено");

            ApplicationUser applicationUser = _dbContext.ApplicationUsers
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
        }
    }
}
