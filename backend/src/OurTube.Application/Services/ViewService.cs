using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class ViewService
    {
        private ApplicationDbContext _dbContext;
        private VideoService _videoService;
        private IMapper _mapper;

        public ViewService(ApplicationDbContext dbContext, VideoService videoService, IMapper mapper)
        {
            _dbContext = dbContext;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task AddVideo(int videoId, string userId, long endTime)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(au => au.Views)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            if(_dbContext.Videos.FirstOrDefault(v => v.Id ==videoId)== null)
                throw new InvalidOperationException("Видео не найдено");

            View view = applicationUser.Views.FirstOrDefault(v => v.VideoId == videoId);

            if (view != null)
            {
                view.DateTime = DateTime.UtcNow;
                view.EndTime = endTime;
            }
            else
            {
                applicationUser.Views.Add(new View()
                {
                   VideoId = videoId,
                   EndTime = endTime
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveVideo(int videoId, string userId)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(au => au.Views)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            if (_dbContext.Videos.FirstOrDefault(v => v.Id == videoId) == null)
                throw new InvalidOperationException("Видео не найдено");

            View view = applicationUser.Views.FirstOrDefault(v => v.VideoId== videoId);

            if (view == null)
                return;

            applicationUser.Views.Remove(view);

            await _dbContext.SaveChangesAsync();
        }

        public async Task ClearHistory(string userId)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(au => au.Views)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            applicationUser.Views.Clear();

            await _dbContext.SaveChangesAsync();
        }

        public List<ViewGetDTO> GetWithLimit(string userId, int limit, int after)
        {
            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(au => au.Views)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            List<View> history = applicationUser.Views
                .OrderBy(pe => pe.DateTime)
                .Skip(after)
                .Take(limit)
                .ToList();

            List<ViewGetDTO> result = new List<ViewGetDTO>();

            foreach(View view in history)
            {
                ViewGetDTO viewDTO = _mapper.Map<ViewGetDTO>(view);
                viewDTO.Video = _videoService.GetVideoById(view.VideoId);
                result.Add(viewDTO);
            }

            return result;

        }
    }
}
