using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class VideoService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;
        private FfmpegProcessor _videoProcessor;
        private MinioService _minioService;
        private VideoValidator _validator;
        private LocalFilesService _localFilesService;

        private readonly int[] _videoResolutions;

        public VideoService(ApplicationDbContext context,
            IMapper mapper,
            FfmpegProcessor videoProcessor,
            IConfiguration configuration,
            VideoValidator validator,
            LocalFilesService localFilesService,
            MinioService minioService)
        {
            _context = context;
            _mapper = mapper;
            _videoProcessor = videoProcessor;
            _minioService = minioService;
            _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
            _validator = validator;
            _localFilesService = localFilesService;

        }

        public VideoDTO GetVideoById(int id)
        {
            Video video = _context.Videos
                .Include(v => v.VideoPreview)
                    .ThenInclude(vp => vp.Bucket)
                .Include(v => v.Files)
                    .ThenInclude(f => f.Bucket)
                .Include(v => v.ApplicationUser)
                    .ThenInclude(u => u.UserAvatars)
                        .ThenInclude(ua => ua.Bucket)
                .First(v => v.Id == id);

            return _mapper.Map<VideoDTO>(video);
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

                Bucket bucket = _context.Buckets.FirstOrDefault(b => b.Name == "videos");
                if (bucket == null)
                {
                    bucket = new Bucket { Name = "videos" };
                    _context.Buckets.Add(bucket);
                }

                ObservableCollection<VideoPlaylist> playlists = new ObservableCollection<VideoPlaylist>();

                Task[] tasks = _videoResolutions.Select(async resolution =>
                {
                    Directory.CreateDirectory(Path.Combine(filePref, resolution.ToString()));

                    VideoPlaylist playlist = new VideoPlaylist()
                    {
                        Resolution = resolution,
                        FileName = Path.Combine(filePref, resolution.ToString(), "playlist.m3u8").Replace(@"\", @"/"),
                        Bucket = bucket
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
                         playlist.Bucket.Name);

                    await _minioService.UploadFiles(
                        Directory.GetFiles(Path.Combine(tempVideoDir, resolution.ToString(), "segments")),
                        playlist.Bucket.Name,
                        Path.Combine(filePref, resolution.ToString(), "segments").Replace(@"\", @"/")
                        );

                    playlists.Add(playlist);
                }).ToArray();
                await Task.WhenAll(tasks);

                // Отправляем превью
                video.VideoPreview = new VideoPreview()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempPreviewPath)).Replace(@"\", @"/"),
                    Bucket = bucket
                };

                await _minioService.UploadFile(
                    tempPreviewPath,
                    video.VideoPreview.FileName,
                    video.VideoPreview.Bucket.Name);

                // Отпавляем изначальное видео
                video.VideoSource = new VideoSource()
                {
                    FileName = Path.Combine(filePref, Path.GetFileName(tempSourcePath)).Replace(@"\", @"/"),
                    Bucket = bucket
                };
                await _minioService.UploadFile(
                    tempSourcePath,
                    video.VideoSource.FileName,
                    video.VideoSource.Bucket.Name);

                video.Files = new List<VideoPlaylist>();

                // Сохраняем сущность видео
                foreach (VideoPlaylist playlist in playlists)
                {
                    video.Files.Add(playlist);
                }

                video.ApplicationUser = _context.ApplicationUsers.Find(userId);

                _context.Videos.Add(video);

                await _context.SaveChangesAsync();
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
