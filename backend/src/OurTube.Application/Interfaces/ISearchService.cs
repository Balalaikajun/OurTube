using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;

namespace OurTube.Application.Interfaces;

public interface ISearchService
{
    Task<ListReply<MinVideo>> SearchVideos(Guid? userId, Guid sessionId, PaginationQueryParametersWithSearch parameters);
}