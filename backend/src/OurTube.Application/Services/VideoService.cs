using System.Collections.Concurrent;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Other;
using System.Collections.ObjectModel;

namespace OurTube.Application.Services
{
    public class VideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FfmpegProcessor _videoProcessor;
        private readonly MinioService _minioService;
        private readonly VideoValidator _validator;

        private readonly int[] _videoResolutions;
        private readonly string _bucket;

        public VideoService(IUnitOfWork unitOfWork,
            IMapper mapper,
            FfmpegProcessor videoProcessor,
            IConfiguration configuration,
            VideoValidator validator,
            LocalFilesService localFilesService,
            MinioService minioService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _videoProcessor = videoProcessor;
            _minioService = minioService;
            _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
            _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
            _validator = validator;
        }

        public async Task<VideoGetDto> GetVideoByIdAsync(int videoId)
        {
            var video =await _unitOfWork.Videos.GetFullVideoDataAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var videoDto = _mapper.Map<VideoGetDto>(video);

            return videoDto;
        }

        public async Task<VideoGetDto> GetVideoByIdAsync(int videoId, string userId)
        {
            var videoDto =await GetVideoByIdAsync(videoId);

            var vote = await _unitOfWork.VideoVotes.GetAsync(videoId, userId);

            if (vote != null)
                videoDto.Vote = vote.Type;

            var view =await _unitOfWork.Views.GetAsync(videoId, userId);

            if (view != null)
                videoDto.EndTime = view.EndTime;

            if (await _unitOfWork.Subscriptions.ContainsAsync( userId,videoDto.User.Id))
                videoDto.User.IsSubscribed = true;

            return videoDto;
        }

        public async Task<VideoMinGetDto> GetMinVideoByIdAsync(int videoId)
        {
            var video = await _unitOfWork.Videos.GetMinVideoDataAsync(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var videoDto = _mapper.Map<VideoMinGetDto>(video);

            return videoDto;
        }

        public async Task<VideoMinGetDto> GetMinVideoByIdAsync(int videoId, string userId)
        {
            var videoDto =await GetMinVideoByIdAsync(videoId);

            var vote =await _unitOfWork.VideoVotes.GetAsync(videoId, userId);

            if (vote != null)
                videoDto.Vote = vote.Type;

            var view =await _unitOfWork.Views.GetAsync(videoId, userId);
            if (view != null)
                videoDto.EndTime = view.EndTime;

            if (await _unitOfWork.Subscriptions.ContainsAsync(userId,videoDto.User.Id))
                videoDto.User.IsSubscribed = true;

            return videoDto;
        }
        
        public async Task PostVideo(
            VideoUploadDto videoUploadDto,
            string userId,
            string segmentsUriPrefix)
        {
            // Валидация
            _validator.ValidateVideo(videoUploadDto);

            var videoDto = videoUploadDto.VideoPostDto;


            var guid = Guid.NewGuid().ToString();
            var tempVideoDir = Path.Combine(Path.GetTempPath() + guid);

            try
            {
                // Сохраняем файлы локально
                if (!Directory.Exists(tempVideoDir))
                {
                    Directory.CreateDirectory(tempVideoDir);
                }
                
                var tempPreviewPath = await LocalFilesService.SaveFileAsync(
                    videoUploadDto.PreviewFile,
                    tempVideoDir,
                    "preview" + Path.GetExtension(videoUploadDto.PreviewFile.FileName));

                var tempSourcePath = await LocalFilesService.SaveFileAsync(
                    videoUploadDto.VideoFile,
                    tempVideoDir,
                    "source" + Path.GetExtension(videoUploadDto.VideoFile.FileName));


                

                // Данные для плейлистов
                var filePref = guid;

                var playlists = new ConcurrentBag<VideoPlaylist>();

                Task[] tasks = _videoResolutions.Select(async resolution =>
                {
                    Directory.CreateDirectory(Path.Combine(filePref, resolution.ToString()));

                    var playlist = new VideoPlaylist()
                    {
                        Resolution = resolution,
                        FileName = Path.Combine(filePref, resolution.ToString(), "playlist.m3u8").Replace(@"\", @"/"),
                        Bucket = _bucket
                    };


                    //Обработка видео
                    await FfmpegProcessor.HandleVideo(
                        tempSourcePath,
                        Path.Combine(tempVideoDir, resolution.ToString()),
                        resolution,
                        segmentsUriPrefix);


                    //Отправка
                    await _minioService.UploadFile(
                        Path.Combine(tempVideoDir, resolution.ToString(), Path.GetFileName(playlist.FileName)),
                        playlist.FileName,
                         playlist.Bucket);

                    await _minioService.UploadFiles(
                        Directory.GetFiles(Path.Combine(tempVideoDir, resolution.ToString(), "segments")),
                        playlist.Bucket,
                        Path.Combine(filePref, resolution.ToString(), "segments").Replace(@"\", @"/")
                        );

                    playlists.Add(playlist);
                }).ToArray();
                await Task.WhenAll(tasks);

                // Отправляем превью
                var preview = new VideoPreview()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempPreviewPath)).Replace(@"\", @"/"),
                    Bucket = _bucket
                };

                await _minioService.UploadFile(
                    tempPreviewPath,
                    preview.FileName,
                    preview.Bucket);

                // Отпавляем изначальное видео
                var source = new VideoSource()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempSourcePath)).Replace(@"\", @"/"),
                    Bucket = _bucket
                };
                await _minioService.UploadFile(
                    tempSourcePath,
                    source.FileName,
                    source.Bucket);

                var files = playlists.ToList();

                var user =await _unitOfWork.ApplicationUsers.GetAsync(userId);

                // Создаём сущность
                var video = new Video(
                    videoDto.Title,
                    videoDto.Description,
                    preview,
                    source,
                    files
                );
                
                _unitOfWork.Videos.Add(video);

                await _unitOfWork.SaveChangesAsync();
            }
            finally
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(tempVideoDir))
                        Directory.Delete(tempVideoDir, true);
                });
            }
        }
    }
}
