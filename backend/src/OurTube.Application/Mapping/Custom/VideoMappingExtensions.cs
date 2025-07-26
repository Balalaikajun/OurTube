using AutoMapper;
using OurTube.Application.Replies.ApplicationUser;
using OurTube.Application.Replies.Video;
using OurTube.Application.Replies.VideoPreview;
using Video = OurTube.Domain.Entities.Video;

namespace OurTube.Application.Mapping.Custom;

public static class VideoMappingExtensions
{
    public static IQueryable<MinVideo> ProjectToMinDto(
        this IQueryable<Video> query,
        IMapper mapper,
        Guid? userId)
    {
        return query.Select(v => new MinVideo()
        {
            Id = v.Id,
            Title = v.Title,
            ViewsCount = v.ViewsCount,
            Duration = v.Duration,
            CreatedDate = v.CreatedDate,
            Vote = userId != null
                ? v.Votes
                    .Where(vv => vv.ApplicationUserId == userId)
                    .Select(vv => (bool?)vv.Type)
                    .FirstOrDefault()
                : null,
            EndTime = userId != null
                ? v.Views
                    .Where(vv => vv.ApplicationUserId == userId)
                    .Select(vv => vv.EndTime)
                    .FirstOrDefault()
                : null,
            Preview = mapper.Map<VideoPreview>(v.Preview),
            User = mapper.Map<ApplicationUser>(v.User)
        });
    }
}