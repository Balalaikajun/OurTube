using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;

namespace OurTube.Application.Services
{
    public class CommentService
    {
        private ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public CommentService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task Create(string userId, CommentPostDTO postDTO)
        {
            Video video = _dbContext.Videos
                .Include(v => v.Comments)
                .FirstOrDefault(v => v.Id == postDTO.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            Comment parent = video.Comments
                .FirstOrDefault(x => x.Id == postDTO.ParentId);

            Comment comment = new Comment()
            {
                ApplicationUserId = userId,
                VideoId = postDTO.VideoId,
                Text = postDTO.Text,
                Parent = parent
            };

            video.Comments.Add(comment);

            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(string userId, CommentPatchDTO postDTO)
        {
            Comment comment = _dbContext.Comments
                .FirstOrDefault(x => x.Id == postDTO.Id);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");


            comment.Text = postDTO.Text;
            comment.Edited = true;


            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int commentId, string userId)
        {
            Comment comment = _dbContext.Comments
                .FirstOrDefault(x => x.Id == commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            _dbContext.Comments.Remove(comment);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CommentGetDTO>> GetWithLimit(int videoId, int limit, int after)
        {
            Video video = await _dbContext.Videos
                .FirstOrDefaultAsync(v => v.Id == videoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            List<Comment> comments = _dbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Childs)
                    .ThenInclude(c => c.User)
                .Where(c => c.ParentId == null)
                .OrderBy(c => c.LikesCount)
                .Skip(after)
                .Take(limit)
                .ToList();

            return _mapper.Map<List<CommentGetDTO>>(comments);

        }

        public async Task<List<CommentGetDTO>> GetChildsWithLimit(int commentId, int limit, int after)
        {
            Comment comment = await _dbContext.Comments
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
                throw new InvalidOperationException("Видео не найдено");

            List<Comment> comments = _dbContext.Comments
                .Include(c => c.User)
                .Include(c => c.Childs)
                    .ThenInclude(c => c.User)
                .Where(c => c.ParentId == commentId)
                .OrderBy(c => c.LikesCount)
                .Skip(after)
                .Take(limit)
                .ToList();

            return _mapper.Map<List<CommentGetDTO>>(comments);

        }


    }
}
