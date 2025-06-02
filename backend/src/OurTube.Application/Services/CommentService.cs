using AutoMapper;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task CreateAsync(string userId, CommentPostDto postDto)
        {
            var video = await _unitOfWork.Videos.GetAsync(postDto.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var parent = await _unitOfWork.Comments
                .GetAsync(postDto.ParentId);

            var comment = new Comment()
            {
                ApplicationUserId = userId,
                VideoId = postDto.VideoId,
                Text = postDto.Text,
                Parent = parent,
                Created = DateTime.Now
            };

            _unitOfWork.Comments.Add(comment);
            video.CommentsCount++;


            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateAsync(string userId, CommentPatchDto postDto)
        {
            var comment =await _unitOfWork.Comments
                .GetAsync(postDto.Id);

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

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment =await _unitOfWork.Comments
                .GetAsync(commentId);

            var video =await _unitOfWork.Videos.GetAsync(comment.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");
            
            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            if (comment.IsDeleted == true)
                throw new InvalidOperationException("Комментарий удалён");

            comment.IsDeleted = true;
            comment.Deleted = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PagedCommentDto> GetChildrenWithLimitAsync(int videoId, int limit, int after, int? parentId = null)
        {
            if (!await _unitOfWork.Videos.ContainsAsync(videoId))
                throw new InvalidOperationException("Видео не найдено");

            if (parentId != null && !await _unitOfWork.Comments.ContainsAsync(parentId))
                throw new InvalidOperationException("Комментарий не найден");

            var comments = await _unitOfWork.Comments
                            .GetWithLimitAsync(videoId, limit+1, after, parentId);

            var hasMore = comments.Count() > after;
            var commentsDto = _mapper.Map<IEnumerable<CommentGetDto>>(comments.Take(limit));
            
            return new PagedCommentDto{ Comments = commentsDto,NextAfter = limit+after, HasMore = hasMore};
        }
        
        public async Task<PagedCommentDto> GetChildrenWithLimitAsync(int videoId, int limit, int after, string userId, int? parentId = null)
        {
            var result = await GetChildrenWithLimitAsync(videoId, limit, after, parentId);

            if(!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                return result;

            var commnetsIds = result.Comments.Select(c => c.Id).ToList();
            var commentVotes = await _unitOfWork.CommentVoices.GetByUserIdAndCommentIdsAsync(userId, commnetsIds);

            foreach (var vote in commentVotes)
            {
                result.Comments.First(c => c.Id == vote.CommentId).Vote = vote.Type;
            }
            
            return result;
        }


    }
}
