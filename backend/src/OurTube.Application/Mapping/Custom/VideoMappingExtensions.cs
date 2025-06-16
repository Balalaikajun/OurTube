using AutoMapper;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.VideoPreview;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping.Custom;

public static class VideoMappingExtensions
{
    public static IQueryable<VideoMinGetDto> ProjectToMinDto(
        this IQueryable<Video> query,
        IMapper mapper,
        string? userId)
    {
        return query.Select(v => new VideoMinGetDto
        {
            Id = v.Id,
            Title = v.Title,
            ViewsCount = v.ViewsCount,
            Duration = v.Duration,
            Created = v.Created,
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
            Preview = mapper.Map<VideoPreviewDto>(v.Preview),
            User = mapper.Map<ApplicationUserDto>(v.User)
        });
    }
}