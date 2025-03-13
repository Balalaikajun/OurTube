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
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;
        private readonly FfmpegProcessor _videoProcessor;
        private readonly MinioService _minioService;
        private readonly VideoValidator _validator;

        private readonly int[] _videoResolutions;
        private readonly string _bucket;

        public VideoService(IUnitOfWorks unitOfWorks,
            IMapper mapper,
            FfmpegProcessor videoProcessor,
            IConfiguration configuration,
            VideoValidator validator,
            LocalFilesService localFilesService,
            MinioService minioService)
        {
            _unitOfWorks = unitOfWorks;
            _mapper = mapper;
            _videoProcessor = videoProcessor;
            _minioService = minioService;
            _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
            _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
            _validator = validator;
        }

        public VideoGetDto GetVideoById(int videoId)
        {
            var video = _unitOfWorks.Videos.GetFullVideoData(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var videoDto = _mapper.Map<VideoGetDto>(video);

            return videoDto;
        }

        public VideoGetDto GetVideoById(int videoId, string userId)
        {
            var videoDto = GetVideoById(videoId);

            var vote = _unitOfWorks.VideoVotes.Get(videoId, userId);

            if (vote != null)
                videoDto.Vote = vote.Type;

            var view = _unitOfWorks.Views.Get(videoId, userId);

            if (view != null)
                videoDto.EndTime = view.EndTime;

            if (_unitOfWorks.Subscriptions.Contains(videoDto.User.Id, userId))
                videoDto.User.IsSubscribed = true;

            return videoDto;
        }

        public VideoMinGetDto GetMinVideoById(int videoId)
        {
            var video = _unitOfWorks.Videos.GetMinVideoData(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var videoDto = _mapper.Map<VideoMinGetDto>(video);

            return videoDto;
        }

        public VideoMinGetDto GetMinVideoById(int videoId, string userId)
        {
            var videoDto = GetMinVideoById(videoId);

            var vote = _unitOfWorks.VideoVotes.Get(videoId, userId);

            if (vote != null)
                videoDto.Vote = vote.Type;

            var view = _unitOfWorks.Views.Get(videoId, userId);
            if (view != null)
                videoDto.EndTime = view.EndTime;

            if (_unitOfWorks.Subscriptions.Find(s => s.SubscribedToId == videoDto.User.Id && s.SubscriberId == userId).FirstOrDefault() != null)
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


                // Создаём сущность
                var video = new Video
                {
                    Title = videoDto.Title,
                    Description = videoDto.Description,
                };

                // Данные для плейлистов
                var filePref = guid;

                var playlists = new ObservableCollection<VideoPlaylist>();

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
                video.Preview = new VideoPreview()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempPreviewPath)).Replace(@"\", @"/"),
                    Bucket = _bucket
                };

                await _minioService.UploadFile(
                    tempPreviewPath,
                    video.Preview.FileName,
                    video.Preview.Bucket);

                // Отпавляем изначальное видео
                video.Source = new VideoSource()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempSourcePath)).Replace(@"\", @"/"),
                    Bucket = _bucket
                };
                await _minioService.UploadFile(
                    tempSourcePath,
                    video.Source.FileName,
                    video.Source.Bucket);

                video.Files = new List<VideoPlaylist>();

                // Сохраняем сущность видео
                foreach (var playlist in playlists)
                {
                    video.Files.Add(playlist);
                }

                video.User = _unitOfWorks.ApplicationUsers.Get(userId);

                _unitOfWorks.Videos.Add(video);

                await _unitOfWorks.SaveChangesAsync();
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
