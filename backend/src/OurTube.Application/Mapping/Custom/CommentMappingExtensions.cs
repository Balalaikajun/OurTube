using AutoMapper;
using OurTube.Domain.Entities;
using ApplicationUser = OurTube.Application.Replies.ApplicationUser.ApplicationUser;

namespace OurTube.Application.Mapping.Custom;

public static class CommentMappingExtensions
{
    public static IQueryable<Replies.Comment.Comment> ProjectToDto(this IQueryable<Comment> query, IMapper mapper, Guid? userId)
    {
        return query.Select(c => new Replies.Comment.Comment()
        {
            Id = c.Id,
            Text = c.IsDeleted ? "" : c.Text,
            CreatedDate = c.CreatedDate,
            UpdatedDate = c.CreatedDate,
            DeletedDate = c.CreatedDate,
            ParentId = c.ParentId,
            IsEdited = c.IsEdited,
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
            User = mapper.Map<ApplicationUser>(c.User)
        });
    }
}