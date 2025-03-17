using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentVoteService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public CommentVoteService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task SetAsync(int commentId, string userId, bool type)
        {
            var comment =await _unitOfWorks.Comments
                .GetAsync(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (!await _unitOfWorks.ApplicationUsers.ContainsAsync(userId))
                throw new InvalidOperationException("Пользователь не найден");

            var vote =await _unitOfWorks.CommentVoices.GetAsync(commentId, userId);

            if (vote == null)
            {
                _unitOfWorks.CommentVoices.Add(new CommentVote()
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

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commentId, string userId)
        {
            var comment =await _unitOfWorks.Comments
                .GetAsync(commentId);


            if (comment == null)
                throw new InvalidOperationException("Комментарий не найдено");

            var vote =await _unitOfWorks.CommentVoices.GetAsync(commentId, userId);

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

            _unitOfWorks.CommentVoices.Remove(vote);

            await _unitOfWorks.SaveChangesAsync();

        }
    }
}

