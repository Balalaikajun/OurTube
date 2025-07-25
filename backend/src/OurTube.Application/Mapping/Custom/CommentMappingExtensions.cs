using AutoMapper;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public static class CommentMappingExtensions
{
    public static IQueryable<CommentGetDto> ProjectToDto(this IQueryable<Comment> query, IMapper mapper, Guid? userId)
    {
        return query.Select(c => new CommentGetDto
        {
            Id = c.Id,
            Text = c.IsDeleted ? "" : c.Text,
            Created = c.CreatedDate,
            Updated = c.CreatedDate,
            Deleted = c.CreatedDate,
            ParentId = c.ParentId,
            IsEdited = c.IsUpdated,
            IsDeleted = c.IsDeleted,
            Vote = userId != null
                ? c.Votes
                    .Where(cv => cv.ApplicationUserId == userId)
                    .Select(cv => (bool?)cv.Type)
                    .FirstOrDefault()
                : null,
            LikesCount = c.LikesCount,
            ChildsCount = c.ChildsCount,
            DislikesCount = c.DislikesCount,
            User = mapper.Map<ApplicationUserDto>(c.User)
        });
    }
}