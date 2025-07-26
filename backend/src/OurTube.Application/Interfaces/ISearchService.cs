using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

namespace OurTube.Application.Interfaces;

public interface ISearchService
{
    Task<ListReply<MinVideo>> SearchVideos(SearchRequest request);
}