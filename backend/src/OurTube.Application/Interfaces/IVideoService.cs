using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IVideoService
{
    Task<VideoGetDto> GetVideoByIdAsync(int videoId);
    Task<VideoGetDto> GetVideoByIdAsync(int videoId, string userId);
    Task<VideoMinGetDto> GetMinVideoByIdAsync(int videoId, string? userId);

    Task<IEnumerable<VideoMinGetDto>> GetVideosByIdAsync(IReadOnlyList<int> videoIds,
        string? userId = null);

    Task<VideoMinGetDto> PostVideo(
        VideoUploadDto videoUploadDto,
        string userId);
}