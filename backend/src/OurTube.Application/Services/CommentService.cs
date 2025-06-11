using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CommentService(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<CommentGetDto> CreateAsync(string userId, CommentPostDto postDto)
        {
            var video = await _dbContext.Videos.FindAsync(postDto.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var parent = await _dbContext.Comments.FindAsync(postDto.ParentId);

            var comment = new Comment()
            {
                ApplicationUserId = userId,
                VideoId = postDto.VideoId,
                Text = postDto.Text,
                Parent = parent,
                Created = DateTime.UtcNow
            };

            _dbContext.Comments.Add(comment);
            video.CommentsCount++;


            await _dbContext.SaveChangesAsync();

            return _mapper.Map<CommentGetDto>(comment);
        }

        public async Task UpdateAsync(string userId, CommentPatchDto postDto)
        {
            var comment = await _dbContext.Comments
                .FindAsync(postDto.Id);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            if (comment.IsDeleted == true)
                throw new InvalidOperationException("Комментарий удалён");

            if (postDto.Text != "")
            {
                comment.Text = postDto.Text;
                comment.IsEdited = true;
                comment.Updated = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment = await _dbContext.Comments
                .FindAsync(commentId);

            var video = await _dbContext.Videos.FindAsync(comment.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            comment.IsDeleted = true;
            comment.Deleted = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PagedCommentDto> GetChildrenWithLimitAsync(int videoId, int limit, int after, string userId = "sadfghjkj",
            int? parentId = null)
        {
            if (!await _dbContext.Videos.AnyAsync(v => v.Id == videoId))
                throw new InvalidOperationException("Видео не найдено");

            if (parentId != null && !await _dbContext.Comments.AnyAsync(p => p.Id == parentId))
                throw new InvalidOperationException("Комментарий не найден");

            var hasUserId = !string.IsNullOrEmpty(userId);
            
            var comments = await _dbContext.Comments
                .Where(c => c.VideoId == videoId && c.ParentId == parentId)
                .OrderByDescending(c => c.LikesCount)
                .Skip(after)
                .Take(limit + 1)
                .Select(c => new CommentGetDto
                {
                    Id = c.Id,
                    Text = c.IsDeleted ? "" : c.Text,
                    Created = c.Created,
                    Updated = c.Updated,
                    Deleted = c.Deleted,
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
                    DislikesCount = c.DislikesCount,
                    User = _mapper.Map<ApplicationUserDto>(c.User)
                })
                .ToListAsync();
            var hasMore = comments.Count > after;
            return new PagedCommentDto
            {
                Comments = comments.Take(limit), NextAfter = limit + after,
                HasMore = hasMore
            };
        }
    }
}