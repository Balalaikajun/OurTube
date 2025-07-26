using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Replies.Views;
using OurTube.Application.Requests.Views;

namespace OurTube.Application.Interfaces;

public interface IViewService
{
    Task AddVideoAsync(PostViewsRequest request, Guid userId);
    Task RemoveVideoAsync(Guid videoId, Guid userId);
    Task ClearHistoryAsync(Guid userId);
    Task<ListReply<MinVideo>> GetWithLimitAsync(Guid userId, int limit, int after, string? query);
}