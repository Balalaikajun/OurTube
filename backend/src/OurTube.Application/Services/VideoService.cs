using System.Collections.Concurrent;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<VideoService> _logger;

    private readonly int[] _videoResolutions;

    public VideoService(IApplicationDbContext dbContext,
        IMapper mapper,
        IVideoProcessor videoProcessor,
        IConfiguration configuration,
        VideoValidator validator,
        IStorageClient storageClient,
        ITagService tagService, ILogger<VideoService> logger)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _videoProcessor = videoProcessor;
        _storageClient = storageClient;
        _videoResolutions = configuration.GetSection("VideoSettings:Resolutions").Get<int[]>();
        _bucket = configuration.GetSection("Minio:VideoBucket").Get<string>();
        _validator = validator;
        _tagService = tagService;
        _logger = logger;
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

        var vote = await _dbContext.VideoVotes.FirstOrDefaultAsync(vv =>
            vv.VideoId == videoId && vv.ApplicationUserId == userId);

        if (vote != null)
            videoDto.Vote = vote.Type;

        var view = await _dbContext.Views.FirstOrDefaultAsync(vv =>
            vv.VideoId == videoId && vv.ApplicationUserId == userId);

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
    
    public async Task<MinVideo> PostVideo(PostVideoRequest request, Guid userId)
    {
        _validator.ValidateVideo(request);

        var guid = Guid.NewGuid().ToString();
        var tempVideoDir = Path.Combine(Path.GetTempPath(), guid);
        var storagePrefix = guid;

        try
        {
            var (tempPreviewPath, tempSourcePath) = await SaveTempFilesAsync(request, tempVideoDir);

            var playlists = await ProcessAndUploadResolutionsAsync(tempSourcePath, tempVideoDir, storagePrefix);

            var (preview, source) = await UploadPreviewAndSourceAsync(tempPreviewPath, tempSourcePath, storagePrefix);

            var tags = new List<VideoTags>();
            foreach (var tagName in request.Tags)
            {
                var tag = await _tagService.GetOrCreate(tagName);
                tags.Add(new VideoTags { TagId = tag.Id });
            }

            var duration = await _videoProcessor.GetVideoDuration(tempSourcePath);

            var video = new Video(
                request.Title,
                request.Description,
                preview,
                source,
                userId,
                playlists,
                tags,
                duration);

            _dbContext.Videos.Add(video);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<MinVideo>(video);
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempVideoDir))
                    Directory.Delete(tempVideoDir, true);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clean up temp directory: {Dir}", tempVideoDir);
            }
        }
    }
    
     private async Task<(string PreviewPath, string SourcePath)> SaveTempFilesAsync(PostVideoRequest request,
        string tempVideoDir)
    {
        if (!Directory.Exists(tempVideoDir))
            Directory.CreateDirectory(tempVideoDir);

        var tempPreviewPath = await LocalFilesService.SaveFileAsync(
            request.PreviewFile,
            tempVideoDir,
            "preview" + Path.GetExtension(request.PreviewFile.FileName));

        var tempSourcePath = await LocalFilesService.SaveFileAsync(
            request.VideoFile,
            tempVideoDir,
            "source" + Path.GetExtension(request.VideoFile.FileName));

        return (tempPreviewPath, tempSourcePath);
    }

    private static string ToStoragePath(string path) =>
        path.Replace(Path.DirectorySeparatorChar, '/');

    private async Task<List<VideoPlaylist>> ProcessAndUploadResolutionsAsync(
        string tempSourcePath,
        string tempVideoDir,
        string storagePrefix)
    {
        var playlists = new List<VideoPlaylist>();

        foreach (var resolution in _videoResolutions)
        {
            var localOutputDir = Path.Combine(tempVideoDir, resolution.ToString());
            Directory.CreateDirectory(localOutputDir);

            var playlist = new VideoPlaylist
            {
                Resolution = resolution,
                FileName = ToStoragePath(Path.Combine(storagePrefix, resolution.ToString(), "playlist.m3u8")),
                Bucket = _bucket
            };

            await _videoProcessor.HandleVideo(tempSourcePath, localOutputDir, resolution, "");

            await _storageClient.UploadFileAsync(
                Path.Combine(localOutputDir, "playlist.m3u8"),
                playlist.FileName,
                playlist.Bucket);

            await _storageClient.UploadFilesAsync(
                Directory.GetFiles(Path.Combine(localOutputDir, "segments")),
                playlist.Bucket,
                ToStoragePath(Path.Combine(storagePrefix, resolution.ToString(), "segments")));

            playlists.Add(playlist);
        }

        return playlists;
    }

    private async Task<(VideoPreview Preview, VideoSource Source)> UploadPreviewAndSourceAsync(
        string tempPreviewPath,
        string tempSourcePath,
        string storagePrefix)
    {
        var preview = new VideoPreview
        {
            FileName = ToStoragePath(Path.Combine(storagePrefix, Path.GetFileName(tempPreviewPath))),
            Bucket = _bucket
        };
        await _storageClient.UploadFileAsync(tempPreviewPath, preview.FileName, preview.Bucket);

        var source = new VideoSource
        {
            FileName = ToStoragePath(Path.Combine(storagePrefix, Path.GetFileName(tempSourcePath))),
            Bucket = _bucket
        };
        await _storageClient.UploadFileAsync(tempSourcePath, source.FileName, source.Bucket);

        return (preview, source);
    }
}