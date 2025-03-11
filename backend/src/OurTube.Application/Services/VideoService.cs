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
        private IUnitOfWorks _unitOfWorks;
        private IMapper _mapper;
        private FfmpegProcessor _videoProcessor;
        private MinioService _minioService;
        private VideoValidator _validator;
        private LocalFilesService _localFilesService;

        private readonly int[] _videoResolutions;
        private readonly string _bucket;

        public VideoService(IUnitOfWorks unitOfWorks,
            IMapper mapper,
            FfmpegProcessor videoProcessor,
            IConfiguration configuration,
            VideoValidator validator,
            LocalFilesService localFilesService,
            MinioService minioService,
            SubscriptionService subscriptionService)
        {
            _unitOfWorks = unitOfWorks;
            _mapper = mapper;
            _videoProcessor = videoProcessor;
            _minioService = minioService;
            _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
            _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
            _validator = validator;
            _localFilesService = localFilesService;
        }

        public VideoGetDTO GetVideoById(int videoId)
        {
            Video video = _unitOfWorks.Videos.GetFullVideoData(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            VideoGetDTO videoDTO = _mapper.Map<VideoGetDTO>(video);

            return videoDTO;
        }

        public VideoGetDTO GetVideoById(int videoId, string userId)
        {
            VideoGetDTO videoDTO = GetVideoById(videoId);

            ApplicationUser applicationUser = _unitOfWorks.ApplicationUsers.Get(userId);

            VideoVote vote = _unitOfWorks.VideoVotes.Get(videoId, userId);

            if (vote != null)
                videoDTO.Vote = vote.Type;

            View view = _unitOfWorks.Views.Get(videoId, userId);

            if (view != null)
                videoDTO.EndTime = view.EndTime;

            if (_unitOfWorks.Subscriptions.Contains(videoDTO.User.Id, userId))
                videoDTO.User.IsSubscribed = true;

            return videoDTO;
        }

        public VideoMinGetDTO GetMinVideoById(int videoId)
        {
            Video video = _unitOfWorks.Videos.GetMinVideoData(videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            VideoMinGetDTO videoDTO = _mapper.Map<VideoMinGetDTO>(video);

            return videoDTO;
        }

        public VideoMinGetDTO GetMinVideoById(int videoId, string userId)
        {
            VideoMinGetDTO videoDTO = GetMinVideoById(videoId);

            ApplicationUser applicationUser = _unitOfWorks.ApplicationUsers.Get(userId);

            VideoVote vote = _unitOfWorks.VideoVotes.Get(videoId, userId);

            if (vote != null)
                videoDTO.Vote = vote.Type;

            View view = _unitOfWorks.Views.Get(videoId, userId);
            if (view != null)
                videoDTO.EndTime = view.EndTime;

            if (_unitOfWorks.Subscriptions.Find(s => s.SubscribedToId == videoDTO.User.Id && s.SubscriberId == userId).FirstOrDefault() != null)
                videoDTO.User.IsSubscribed = true;

            return videoDTO;
        }



        public async Task PostVideo(
            VideoUploadDTO videoUploadDTO,
            string userId,
            string segmentsUriPrefix)
        {
            // Валидация
            _validator.ValidateVideo(videoUploadDTO);

            VideoPostDTO videoDTO = videoUploadDTO.VideoPostDTO;


            string guid = Guid.NewGuid().ToString();
            string tempVideoDir = Path.Combine(Path.GetTempPath() + guid);

            try
            {
                // Сохраняем файлы локально
                if (!Directory.Exists(tempVideoDir))
                {
                    Directory.CreateDirectory(tempVideoDir);
                }

                string tempPreviewPath = await _localFilesService.SaveFileAsync(
                    videoUploadDTO.PreviewFile,
                    tempVideoDir,
                    "preview" + Path.GetExtension(videoUploadDTO.PreviewFile.FileName));

                string tempSourcePath = await _localFilesService.SaveFileAsync(
                    videoUploadDTO.VideoFile,
                    tempVideoDir,
                    "source" + Path.GetExtension(videoUploadDTO.VideoFile.FileName));


                // Создаём сущность
                Video video = new Video
                {
                    Title = videoDTO.Title,
                    Description = videoDTO.Description,
                };

                // Данные для плейлистов
                string filePref = guid;

                ObservableCollection<VideoPlaylist> playlists = new ObservableCollection<VideoPlaylist>();

                Task[] tasks = _videoResolutions.Select(async resolution =>
                {
                    Directory.CreateDirectory(Path.Combine(filePref, resolution.ToString()));

                    VideoPlaylist playlist = new VideoPlaylist()
                    {
                        Resolution = resolution,
                        FileName = Path.Combine(filePref, resolution.ToString(), "playlist.m3u8").Replace(@"\", @"/"),
                        Bucket = _bucket
                    };


                    //Обработка видео
                    await _videoProcessor.HandleVideo(
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
                foreach (VideoPlaylist playlist in playlists)
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
