using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class CommentVoteService
    {
        private ApplicationDbContext _dbContext;

        public CommentVoteService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Set(int commentId, string userId, bool type)
        {
            Comment comment = _dbContext.Comments
                .FirstOrDefault(v => v.Id == commentId);

            if (comment == null)
                throw new InvalidOperationException("Комментарий не найден");

            ApplicationUser applicationUser = _dbContext.ApplicationUsers
                .Include(au => au.CommentVotes)
                .FirstOrDefault(au => au.Id == userId);

            if (applicationUser == null)
                throw new InvalidOperationException("Пользователь не найден");

            CommentVote vote = applicationUser.CommentVotes
                .FirstOrDefault(v => v.CommentId == commentId);

            if (vote == null)
            {
                _dbContext.CommentVotes.Add(new CommentVote()
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

                if(type == true)
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

            await _dbContext.SaveChangesAsync();
        }

        public async Task Delete(int commentId, string userId)
        {
            Comment comment = _dbContext.Comments
                .Include(c => c.Votes)
                .FirstOrDefault(c => c.Id == commentId);


            if (comment == null)
                throw new InvalidOperationException("Комментарий не найдено");

            CommentVote vote = comment
                .Votes
                .FirstOrDefault(v => v.ApplicationUserId == userId);

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

            _dbContext.CommentVotes.Remove(vote);

            await _dbContext.SaveChangesAsync();

        }
    }
}

