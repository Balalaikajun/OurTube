using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Interfaces;

public interface IViewService
{
    Task AddVideoAsync(int videoId, string userId, TimeSpan endTime);
    Task RemoveVideoAsync(int videoId, string userId);
    Task ClearHistoryAsync(string userId);
    Task<PagedVideoDto> GetWithLimitAsync(string userId, int limit, int after);
}