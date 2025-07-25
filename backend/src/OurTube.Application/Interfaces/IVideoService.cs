using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IVideoService
{
    Task<VideoGetDto> GetVideoByIdAsync(Guid videoId);
    Task<VideoGetDto> GetVideoByIdAsync(Guid videoId, Guid userId);
    Task<VideoMinGetDto> GetMinVideoByIdAsync(Guid videoId, Guid? userId);

    Task<IEnumerable<VideoMinGetDto>> GetVideosByIdAsync(IReadOnlyList<Guid> videoIds,
        Guid? userId = null);

    Task<VideoMinGetDto> PostVideo(
        VideoUploadDto videoUploadDto,
        Guid userId);
}