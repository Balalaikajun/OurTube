using System.Collections.Concurrent;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;
using Video = OurTube.Domain.Entities.Video;

namespace OurTube.Application.Services;

public class VideoService : IVideoService
{
    private readonly IStorageClient _storageClient;
    private readonly string _bucket;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ITagService _tagService;
    private readonly VideoValidator _validator;
    private readonly IVideoProcessor _videoProcessor;

    private readonly int[] _videoResolutions;

    public VideoService(IApplicationDbContext dbContext,
        IMapper mapper,
        IVideoProcessor videoProcessor,
        IConfiguration configuration,
        VideoValidator validator,
        IStorageClient storageClient,
        ITagService tagService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _videoProcessor = videoProcessor;
        _storageClient = storageClient;
        _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
        _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
        _validator = validator;
        _tagService = tagService;
    }

    public async Task<Replies.Video.Video> GetVideoByIdAsync(Guid videoId)
    {
        var video = await _dbContext.Videos
            .ProjectTo<Replies.Video.Video>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(v => v.Id == videoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        return video;
    }

    public async Task<Replies.Video.Video> GetVideoByIdAsync(Guid videoId, Guid userId)
    {
        var videoDto = await GetVideoByIdAsync(videoId);

        var vote = await _dbContext.VideoVotes.FindAsync(videoId, userId);

        if (vote != null)
            videoDto.Vote = vote.Type;

        var view = await _dbContext.Views.FindAsync(videoId, userId);

        if (view != null)
            videoDto.EndTime = view.EndTime;

        if (await _dbContext.Subscriptions.AnyAsync(s =>
                s.SubscriberId == userId && s.SubscribedToId == videoDto.User.Id))
            videoDto.User.IsSubscribed = true;

        return videoDto;
    }

    public async Task<MinVideo> GetMinVideoByIdAsync(Guid videoId, Guid? userId)
    {
        var dto = await _dbContext.Videos
            .Where(v => v.Id == videoId)
            .ProjectToMinDto(_mapper, userId)
            .FirstOrDefaultAsync();

        if (dto == null)
            throw new InvalidOperationException("Видео не найдено");

        return dto;
    }

    public async Task<IEnumerable<MinVideo>> GetVideosByIdAsync(IEnumerable<Guid> videoIds,
        Guid? userId = null)
    {
        var videos = await _dbContext.Videos
            .Where(v => videoIds.Contains(v.Id))
            .ProjectToMinDto(_mapper, userId)
            .ToDictionaryAsync(x => x.Id);

        var result = videoIds.Select(id => videos[id]).ToList();

        return result;
    }

    public async Task<MinVideo> PostVideo(
        PostVideoRequest request,
        Guid userId)
    {
        // Валидация
        _validator.ValidateVideo(request);

        var guid = Guid.NewGuid().ToString();
        var tempVideoDir = Path.Combine(Path.GetTempPath() + guid);

        try
        {
            // Сохраняем файлы локально
            if (!Directory.Exists(tempVideoDir)) Directory.CreateDirectory(tempVideoDir);

            var tempPreviewPath = await LocalFilesService.SaveFileAsync(
                request.PreviewFile,
                tempVideoDir,
                "preview" + Path.GetExtension(request.PreviewFile.FileName));

            var tempSourcePath = await LocalFilesService.SaveFileAsync(
                request.VideoFile,
                tempVideoDir,
                "source" + Path.GetExtension(request.VideoFile.FileName));


            // Данные для плейлистов
            var filePref = guid;

            var playlists = new ConcurrentBag<VideoPlaylist>();

            Task[] tasks = _videoResolutions.Select(async resolution =>
            {
                Directory.CreateDirectory(Path.Combine(filePref, resolution.ToString()));

                var playlist = new VideoPlaylist
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
                    "");


                //Отправка
                await _storageClient.UploadFileAsync(
                    Path.Combine(tempVideoDir, resolution.ToString(), Path.GetFileName(playlist.FileName)),
                    playlist.FileName,
                    playlist.Bucket);

                await _storageClient.UploadFilesAsync(
                    Directory.GetFiles(Path.Combine(tempVideoDir, resolution.ToString(), "segments")),
                    playlist.Bucket,
                    Path.Combine(filePref, resolution.ToString(), "segments").Replace(@"\", @"/")
                );

                playlists.Add(playlist);
            }).ToArray();
            await Task.WhenAll(tasks);

            // Отправляем превью
            var preview = new VideoPreview
            {
                FileName = Path.Combine(filePref, Path.GetFileName(tempPreviewPath)).Replace(@"\", @"/"),
                Bucket = _bucket
            };

            await _storageClient.UploadFileAsync(
                tempPreviewPath,
                preview.FileName,
                preview.Bucket);

            // Отпавляем изначальное видео
            var source = new VideoSource
            {
                FileName = Path.Combine(filePref, Path.GetFileName(tempSourcePath)).Replace(@"\", @"/"),
                Bucket = _bucket
            };
            await _storageClient.UploadFileAsync(
                tempSourcePath,
                source.FileName,
                source.Bucket);

            var files = playlists.ToList();

            var tags = new List<VideoTags>();

            foreach (var tagName in request.Tags)
            {
                var tag = await _tagService.GetOrCreate(tagName);

                tags.Add(new VideoTags{TagId = tag.Id});
            }

            var duration = await _videoProcessor.GetVideoDuration(tempSourcePath);

            // Создаём сущность
            var video = new Video(
                request.Title,
                request.Description,
                preview,
                source,
                userId,
                files,
                tags,
                duration
            );

            _dbContext.Videos.Add(video);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<MinVideo>(video);
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