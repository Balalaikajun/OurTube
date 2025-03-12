using AutoMapper;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;

        public CommentService(IUnitOfWorks unitOfWorks, IMapper mapper)
        {
            _unitOfWorks = unitOfWorks;
            _mapper = mapper;
        }

        public async Task Create(string userId, CommentPostDto postDto)
        {
            var video = _unitOfWorks.Videos.Get(postDto.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var parent = _unitOfWorks.Comments
                .Get(postDto.ParentId);

            var comment = new Comment()
            {
                ApplicationUserId = userId,
                VideoId = postDto.VideoId,
                Text = postDto.Text,
                Parent = parent
            };

            _unitOfWorks.Comments.Add(comment);
            video.CommentsCount++;


            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task Update(string userId, CommentPatchDto postDto)
        {
            var comment = _unitOfWorks.Comments
                .Get(postDto.Id);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            if (postDto.Text != "")
            {
                comment.Text = postDto.Text;
                comment.Edited = true;

                await _unitOfWorks.SaveChangesAsync();
            }
        }

        public async Task Delete(int commentId, string userId)
        {
            var comment = _unitOfWorks.Comments
                .Get(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            var video = _unitOfWorks.Videos.Get(comment.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");
            
            _unitOfWorks.Comments.Remove(comment);
            video.CommentsCount--;

            await _unitOfWorks.SaveChangesAsync();
        }

        public List<CommentGetDto> GetChildsWithLimit(int videoId, int limit, int after, int? parentId = null)
        {
            if (!_unitOfWorks.Videos.Contains(videoId))
                throw new InvalidOperationException("Видео не найдено");

            if (parentId != null && !_unitOfWorks.Comments.Contains(parentId))
                throw new InvalidOperationException("Комментарий не найден");

            var comments = _unitOfWorks.Comments
                            .GetWithLimit(videoId, limit, after, parentId).ToList();

            return _mapper.Map<List<CommentGetDto>>(comments);
        }


    }
}
