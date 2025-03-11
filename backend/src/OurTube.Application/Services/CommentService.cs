using AutoMapper;
using OurTube.Application.DTOs.Comment;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentService
    {
        private IUnitOfWorks _unitOfWroks;
        private IMapper _mapper;

        public CommentService(IUnitOfWorks unitOfWorks, IMapper mapper)
        {
            _unitOfWroks = unitOfWorks;
            _mapper = mapper;
        }

        public async Task Create(string userId, CommentPostDTO postDTO)
        {
            Video video = _unitOfWroks.Videos.Get(postDTO.VideoId);

            if (video == null)
                throw new InvalidOperationException("Видео не найдено");

            Comment parent = _unitOfWroks.Comments
                .Get(postDTO.ParentId);

            Comment comment = new Comment()
            {
                ApplicationUserId = userId,
                VideoId = postDTO.VideoId,
                Text = postDTO.Text,
                Parent = parent
            };

            _unitOfWroks.Comments.Add(comment);
            video.CommentsCount++;


            await _unitOfWroks.SaveChangesAsync();
        }

        public async Task Update(string userId, CommentPatchDTO postDTO)
        {
            Comment comment = _unitOfWroks.Comments
                .Get(postDTO.Id);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            if (postDTO.Text != "")
            {
                comment.Text = postDTO.Text;
                comment.Edited = true;

                await _unitOfWroks.SaveChangesAsync();
            }
        }

        public async Task Delete(int commentId, string userId)
        {
            Comment comment = _unitOfWroks.Comments
                .Get(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (comment.ApplicationUserId != userId)
                throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

            _unitOfWroks.Comments.Remove(comment);
            _unitOfWroks.Videos.Get(comment.VideoId).CommentsCount--;

            await _unitOfWroks.SaveChangesAsync();
        }

        public async Task<List<CommentGetDTO>> GetChildsWithLimit(int videoId, int limit, int after, int? parentId = null)
        {
            if (!_unitOfWroks.Videos.Contains(videoId))
                throw new InvalidOperationException("Видео не найдено");

            if (parentId != null && !_unitOfWroks.Comments.Contains(parentId))
                throw new InvalidOperationException("Комментарий не найден");

            List<Comment> comments = _unitOfWroks.Comments
                            .GetWithLimit(videoId, limit, after, parentId).ToList();

            return _mapper.Map<List<CommentGetDTO>>(comments);

        }


    }
}
