using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.Views;

namespace OurTube.Application.Interfaces;

public interface IViewService
{
    Task AddVideoAsync(ViewPostDto dto, Guid userId);
    Task RemoveVideoAsync(Guid videoId, Guid userId);
    Task ClearHistoryAsync(Guid userId);
    Task<PagedVideoDto> GetWithLimitAsync(Guid userId, int limit, int after, string? query);
}