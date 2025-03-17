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

        public async Task CreateAsync(string userId, CommentPostDto postDto)
        {
            var video = await _unitOfWorks.Videos.GetAsync(postDto.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            var parent = await _unitOfWorks.Comments
                .GetAsync(postDto.ParentId);

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

        public async Task UpdateAsync(string userId, CommentPatchDto postDto)
        {
            var comment =await _unitOfWorks.Comments
                .GetAsync(postDto.Id);

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

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment =await _unitOfWorks.Comments
                .GetAsync(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            var video =await _unitOfWorks.Videos.GetAsync(comment.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");
            
            _unitOfWorks.Comments.Remove(comment);
            video.CommentsCount--;

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task<List<CommentGetDto>> GetChildsWithLimitAsync(int videoId, int limit, int after, int? parentId = null)
        {
            if (!await _unitOfWorks.Videos.ContainsAsync(videoId))
                throw new InvalidOperationException("Видео не найдено");

            if (parentId != null && !await _unitOfWorks.Comments.ContainsAsync(parentId))
                throw new InvalidOperationException("Комментарий не найден");

            var comments = await _unitOfWorks.Comments
                            .GetWithLimitAsync(videoId, limit, after, parentId);

            return _mapper.Map<List<CommentGetDto>>(comments);
        }


    }
}
