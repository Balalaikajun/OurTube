using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

namespace OurTube.Application.Interfaces;

public interface IVideoService
{
    Task<Video> GetVideoByIdAsync(Guid videoId);
    Task<Video> GetVideoByIdAsync(Guid videoId, Guid userId);
    Task<MinVideo> GetMinVideoByIdAsync(Guid videoId, Guid? userId);

    Task<IEnumerable<MinVideo>> GetVideosByIdAsync(IEnumerable<Guid> videoIds,
        Guid? userId = null);

    Task<MinVideo> PostVideo(
        PostVideoRequest request,
        Guid userId);
}