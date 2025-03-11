using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class CommentVoteService
    {
        private IUnitOfWorks _unitOfWorks;

        public CommentVoteService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task Set(int commentId, string userId, bool type)
        {
            Comment comment = _unitOfWorks.Comments
                .Get(commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            if (!_unitOfWorks.ApplicationUsers.Contains(userId))
                throw new InvalidOperationException("Пользователь не найден");

            CommentVote vote = _unitOfWorks.CommentVoices.Get(commentId, userId);

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

        public async Task Delete(int commentId, string userId)
        {
            Comment comment = _unitOfWorks.Comments
                .Get(commentId);


            if (comment == null)
                throw new InvalidOperationException("Комментарий не найдено");

            CommentVote vote = _unitOfWorks.CommentVoices.Get(commentId, userId);

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

