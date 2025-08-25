using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;
using OurTube.Application.Requests.Views;

namespace OurTube.Application.Interfaces;

public interface IViewService
{
    Task AddVideoAsync(Guid userId, Guid videoId, PostViewsRequest request);
    Task RemoveVideoAsync(Guid videoId, Guid userId);
    Task ClearHistoryAsync(Guid userId);
    Task<ListReply<MinVideo>> GetWithLimitAsync(Guid userId, PaginationQueryParametersWithSearch parameter);
}