using AutoMapper;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly VideoService _videoService;
        private readonly IMapper _mapper;

        public PlaylistService(IUnitOfWorks unitOfWork, VideoService videoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _videoService = videoService;
            _mapper = mapper;
        }

        public async Task Create(PlaylistPostDto playlistDto, string userId)
        {
            var playlist = new Playlist()
            {
                Title = playlistDto.Title,
                Description = playlistDto.Description,
                ApplicationUserId = userId
            };

            _unitOfWork.Playlists.Add(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(PlaylistPatchDto patchDto, int playlistId, string userId)
        {
            var playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (patchDto.Title != null)
                playlist.Title = patchDto.Title;

            if (patchDto.Description != null)
                playlist.Description = patchDto.Description;

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(int playlistId, string userId)
        {
            var playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            _unitOfWork.Playlists.Remove(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddVideo(int playlistId, int videoId, string userId)
        {
            var playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var element = new PlaylistElement()
            {
                VideoId = videoId,
                PlaylistId = playlistId
            };

            _unitOfWork.PlaylistElements.Add(element);

            playlist.Count++;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveVideo(int playlistId, int videoId, string userId)
        {
            var playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var playlistElement = _unitOfWork.PlaylistElements.Get(playlistId, videoId);

            if (playlistElement == null)
                return;

            _unitOfWork.PlaylistElements.Remove(playlistElement);
            playlist.Count--;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PlaylistGetDto> GetWithLimit(int playlistId, string userId, int limit, int after)
        {
            var playlist = await _unitOfWork.Playlists.GetPlaylistWithElements(playlistId, limit, after);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            var playlistGetDto = _mapper.Map<PlaylistGetDto>(playlist);



            playlistGetDto.PlaylistElements = new List<PlaylistElementGetDto>();
            foreach (var pe in playlist.PlaylistElements)
            {
                playlistGetDto.PlaylistElements.Add(new PlaylistElementGetDto()
                {
                    AddedAt = pe.AddedAt,
                    Video = _videoService.GetMinVideoById(pe.VideoId, userId)

                });
            }

            return playlistGetDto;

        }

        public IEnumerable<PlaylistMinGetDto> GetUserPlaylists(string userId)
        {
            return _unitOfWork.Playlists.Find(p => p.ApplicationUserId == userId)
                .Select(p => _mapper.Map<PlaylistMinGetDto>(p));
        }

    }
}
