using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentVoteService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentVoteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SetAsync(int commentId, string userId, bool type)
        {
            var comment =await _unitOfWork.Comments
                .GetAsync(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (!await _unitOfWork.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWork.CommentVoices.GetAsync(commentId, userId);

            if (vote == null)
            {
                _unitOfWork.CommentVoices.Add(new CommentVote()
                {
                    ApplicationUserId = userId,
                    CommentId = commentId,
                    Type = type
                });

                if (type == true)
                {
                    comment.LikesCount++;
                }
                else
                {
                    comment.DisLikesCount++;
                }
            }
            else if (vote.Type != type)
            {
                vote.Type = type;

                if (type == true)
                {
                    comment.DisLikesCount--;
                    comment.LikesCount++;
                }
                else
                {
                    comment.DisLikesCount++;
                    comment.LikesCount--;

                }
            }
            else
            {
                return;
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment =await _unitOfWork.Comments
                .GetAsync(commentId);


            if (comment == null)
                throw new InvalidOperationException("Комментарий не найдено");

            var vote =await _unitOfWork.CommentVoices.GetAsync(commentId, userId);

            if (vote == null)
                return;

            if (vote.Type == true)
            {
                comment.LikesCount--;
            }
            else
            {
                comment.DisLikesCount--;
            }

            _unitOfWork.CommentVoices.Remove(vote);

            await _unitOfWork.SaveChangesAsync();

        }
    }
}

