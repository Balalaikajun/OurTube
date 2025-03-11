using AutoMapper;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private IUnitOfWorks _unitOfWork;
        private VideoService _videoService;
        private IMapper _mapper;

        public PlaylistService(IUnitOfWorks unitOfWork, VideoService videoService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
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

            _unitOfWork.Playlists.Add(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task Update(PlaylistPatchDTO patchDTO, int playlistId, string userId)
        {
            Playlist playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            if (patchDTO.Title != null)
                playlist.Title = patchDTO.Title;

            if (patchDTO.Description != null)
                playlist.Description = patchDTO.Description;

            await _unitOfWork.SaveChangesAsync();
        }
        public async Task Delete(int playlistId, string userId)
        {
            Playlist playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            _unitOfWork.Playlists.Remove(playlist);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddVideo(int playlistId, int videoId, string userId)
        {
            Playlist playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            PlaylistElement element = new PlaylistElement()
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
            Playlist playlist = _unitOfWork.Playlists.Get(playlistId);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            PlaylistElement playlistElement = _unitOfWork.PlaylistElements.Get(playlistId, videoId);

            if (playlistElement == null)
                return;

            _unitOfWork.PlaylistElements.Remove(playlistElement);
            playlist.Count--;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PlaylistGetDTO> GetWithLimit(int playlistId, string userId, int limit, int after)
        {
            Playlist playlist = await _unitOfWork.Playlists.GetPlaylistWithElements(playlistId, limit, after);

            if (playlist == null)
                throw new KeyNotFoundException("Плейлист не найден");

            if (playlist.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного плейлиста");

            PlaylistGetDTO playlistGetDTO = _mapper.Map<PlaylistGetDTO>(playlist);



            playlistGetDTO.PlaylistElements = new List<PlaylistElementGetDTO>();
            foreach (PlaylistElement pe in playlist.PlaylistElements)
            {
                playlistGetDTO.PlaylistElements.Add(new PlaylistElementGetDTO()
                {
                    AddedAt = pe.AddedAt,
                    Video = _videoService.GetMinVideoById(pe.VideoId, userId)

                });
            }

            return playlistGetDTO;

        }

        public IEnumerable<PlaylistMinGetDTO> GetUserPlaylists(string userId)
        {
            return _unitOfWork.Playlists.Find(p => p.ApplicationUserId == userId)
                .Select(p => _mapper.Map<PlaylistMinGetDTO>(p));
        }

    }
}
