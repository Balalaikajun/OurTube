using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.Views;

namespace OurTube.Application.Interfaces;

public interface IViewService
{
    Task AddVideoAsync(ViewPostDto dto, string userId);
    Task RemoveVideoAsync(int videoId, string userId);
    Task ClearHistoryAsync(string userId);
    Task<PagedVideoDto> GetWithLimitAsync(string userId, int limit, int after, string? query);
}