using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.VideoPlaylist;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private ApplicationDbContext _dbContext;
        private VideoService _videoService;
        private IMapper _mapper;

        public PlaylistService(ApplicationDbContext dbContext,VideoService videoService  ,IMapper mapper)
        {
            _dbContext = dbContext;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task Create(PlaylistPostDTO playlistDTO, string userId)
        {
            Playlist playlist = new Playlist()
            {
                Title = playlistDTO.Title,
                Description = playlistDTO.Description,
                ApplicationUserId = userId
            };

            _dbContext.Playlists.Add(playlist);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(PlaylistPatchDTO patchDTO, int id, string userId)
        {
            Playlist playlist = _dbContext.Playlists
                .Include(p => p.ApplicationUser)
                .FirstOrDefault(x => x.Id == id);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUser.Id != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (patchDTO.Title != null)
                playlist.Title = patchDTO.Title;

            if (patchDTO.Description != null)
                playlist.Description = patchDTO.Description;

            await _dbContext.SaveChangesAsync();
        }
        public async Task Delete( int id, string userId)
        {
            Playlist playlist = _dbContext.Playlists
                .Include(p => p.ApplicationUser)
                .FirstOrDefault(x => x.Id == id);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUser.Id != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            _dbContext.Playlists.Remove(playlist);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AddVideo(int id,int videoId, string userId)
        {
            Playlist playlist = _dbContext
                .Playlists
                .Include(p => p.PlaylistElements)
                .Include(p => p.ApplicationUser)
                .FirstOrDefault(x => x.Id == id);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUser.Id != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (_dbContext.Videos.FirstOrDefault(v => v.Id == videoId) == null)
                throw new InvalidOperationException("Видео не найдено");

            if (playlist.PlaylistElements.Any(pe => pe.VideoId == videoId))
                throw new InvalidOperationException("Видео уже добавлено в плейлист");


            playlist.PlaylistElements.Add(new PlaylistElement() 
            {
                VideoId = videoId,
                PlaylistId = id
            });
            playlist.Count++;

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveVideo(int id, int videoId, string userId)
        {
            Playlist playlist = _dbContext.Playlists
                .Include(p => p.ApplicationUser)
                .Include(p => p.PlaylistElements)
                .FirstOrDefault(x => x.Id == id);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUser.Id != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            PlaylistElement playlistElement = playlist.PlaylistElements
                .FirstOrDefault(x => x.VideoId == videoId);

            if(playlistElement == null)
                throw new KeyNotFoundException("Видео не найдено в плейлисте");

            _dbContext.PlaylistElements.Remove(playlistElement);
            playlist.Count--;

            await _dbContext.SaveChangesAsync();
        }

        public PlaylistGetDTO GetWithLimit(int id, string userId, int limit, int after)
        {
            Playlist playlist = _dbContext.Playlists
                .Include(p => p.ApplicationUser)
                .Include(p => p.PlaylistElements)
                    .ThenInclude(pe => pe.Video)
                .FirstOrDefault(x => x.Id == id);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUser.Id != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            PlaylistGetDTO playlistGetDTO = _mapper.Map<PlaylistGetDTO>(playlist);

            playlistGetDTO.PlaylistElements = playlistGetDTO.PlaylistElements
                .OrderBy(pe => pe.AddedAt)
                .Skip(after)
                .Take(limit)
                .Select(pe =>
                {
                pe.Video = _videoService.GetVideoById(pe.Video.Id);
                    return pe;
                })
                .ToList();

            return playlistGetDTO;

        }

        public List<PlaylistMinGetDTO> GetUserPlaylists(string userId)
        {
            return _dbContext.Playlists
                .Where(p => p.ApplicationUserId == userId)
                .Select(p => _mapper.Map<PlaylistMinGetDTO>(p))
                .ToList();
        }

    }
}
