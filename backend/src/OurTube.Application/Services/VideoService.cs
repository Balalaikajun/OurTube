using System.Collections.Concurrent;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class VideoService
{
    private readonly IBlobService _blobService;
    private readonly string _bucket;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly TagService _tagService;
    private readonly VideoValidator _validator;
    private readonly IVideoProcessor _videoProcessor;

    private readonly int[] _videoResolutions;

    public VideoService(IApplicationDbContext dbContext,
        IMapper mapper,
        IVideoProcessor videoProcessor,
        IConfiguration configuration,
        VideoValidator validator,
        LocalFilesService localFilesService,
        IBlobService blobService,
        TagService tagService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _videoProcessor = videoProcessor;
        _blobService = blobService;
        _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
        _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
        _validator = validator;
        _tagService = tagService;
    }

    public async Task<VideoGetDto> GetVideoByIdAsync(int videoId)
    {
        var video = await _dbContext.Videos
            .ProjectTo<VideoGetDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(v => v.Id == videoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        return video;
    }

    public async Task<VideoGetDto> GetVideoByIdAsync(int videoId, string userId)
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

    public async Task<VideoMinGetDto> GetMinVideoByIdAsync(int videoId)
    {
        var video = await _dbContext.Videos.ProjectTo<VideoMinGetDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(v => v.Id == videoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        var videoDto = _mapper.Map<VideoMinGetDto>(video);

        return videoDto;
    }

    public async Task<VideoMinGetDto> GetMinVideoByIdAsync(int videoId, string userId)
    {
        var videoDto = await GetMinVideoByIdAsync(videoId);

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

    public async Task<IEnumerable<VideoMinGetDto>> GetVideosByIdAsync(IReadOnlyList<int> videoIds,
        string? userId = null)
    {
        var videos = await _dbContext.Videos
            .Where(v => videoIds.Contains(v.Id))
            .ProjectTo<VideoMinGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        if (string.IsNullOrEmpty(userId))
            return videos;

        var votes = await _dbContext.VideoVotes
            .Where(v => videoIds.Contains(v.VideoId) && v.ApplicationUserId == userId)
            .ToListAsync();

        var views = await _dbContext.Views
            .Where(v => videoIds.Contains(v.VideoId) && v.ApplicationUserId == userId)
            .ToListAsync();

        var authorIds = videos.Select(v => v.User.Id).Distinct().ToList();
        var subs = await _dbContext.Subscriptions
            .Where(s => s.SubscriberId == userId
                        && authorIds.Contains(s.SubscribedToId))
            .Select(s => s.SubscribedToId)
            .ToListAsync();

        var votesByVideo = votes.ToDictionary(v => v.VideoId, v => v.Type);
        var viewsByVideo = views.ToDictionary(v => v.VideoId, v => v.EndTime);

        foreach (var dto in videos)
        {
            if (votesByVideo.TryGetValue(dto.Id, out var vote))
                dto.Vote = vote;

            if (viewsByVideo.TryGetValue(dto.Id, out var endTime))
                dto.EndTime = endTime;

            if (subs.Contains(dto.User.Id))
                dto.User.IsSubscribed = true;
        }

        return videos;
    }

    public async Task<VideoMinGetDto> PostVideo(
        VideoUploadDto videoUploadDto,
        string userId)
    {
        // Валидация
        _validator.ValidateVideo(videoUploadDto);

        var videoDto = videoUploadDto.VideoPostDto;


        var guid = Guid.NewGuid().ToString();
        var tempVideoDir = Path.Combine(Path.GetTempPath() + guid);

        try
        {
            // Сохраняем файлы локально
            if (!Directory.Exists(tempVideoDir)) Directory.CreateDirectory(tempVideoDir);

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
                await _blobService.UploadFile(
                    Path.Combine(tempVideoDir, resolution.ToString(), Path.GetFileName(playlist.FileName)),
                    playlist.FileName,
                    playlist.Bucket);

                await _blobService.UploadFiles(
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

            await _blobService.UploadFile(
                tempPreviewPath,
                preview.FileName,
                preview.Bucket);

            // Отпавляем изначальное видео
            var source = new VideoSource
            {
                FileName = Path.Combine(filePref, Path.GetFileName(tempSourcePath)).Replace(@"\", @"/"),
                Bucket = _bucket
            };
            await _blobService.UploadFile(
                tempSourcePath,
                source.FileName,
                source.Bucket);

            var files = playlists.ToList();

            var tags = new List<VideoTags>();

            foreach (var tagName in videoDto.Tags)
            {
                var tag = await _tagService.GetOrCreate(tagName);

                tags.Add(new VideoTags(tag.Id));
            }

            var duration = await _videoProcessor.GetVideoDuration(tempSourcePath);

            // Создаём сущность
            var video = new Video(
                videoDto.Title,
                videoDto.Description,
                preview,
                source,
                userId,
                files,
                tags,
                duration
            );

            _dbContext.Videos.Add(video);

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<VideoMinGetDto>(video);
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