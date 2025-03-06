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
    public class SubscriptionService
    {
        private ApplicationDbContext _dbContext;

        public SubscriptionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Subscribe(string userId, string userToId)
        {
            ApplicationUser user = _dbContext.ApplicationUsers
                .Include(u => u.SubscribedTo)
                .FirstOrDefault(u => u.Id ==userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            ApplicationUser userTo = _dbContext.ApplicationUsers
                .FirstOrDefault(u => u.Id == userToId);

            if (user == null)
                throw new InvalidOperationException("Канал не найден");

            if(user.Id == userTo.Id)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            Subscription subscription = user.SubscribedTo
                .FirstOrDefault(u => u.SubscribedToId == userToId);

            if (subscription != null)
                return;

            subscription = new Subscription()
            {
                SubscriberId = userId,
                SubscribedToId = userToId
            };

            user.SubscribedTo.Add(subscription);

            user.SubscribedToCount++;
            userTo.SubscribersCount++;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UnSubscribe(string userId, string userToId)
        {
            ApplicationUser user = _dbContext.ApplicationUsers
                .Include(u => u.SubscribedTo)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            ApplicationUser userTo = _dbContext.ApplicationUsers
                .FirstOrDefault(u => u.Id == userToId);

            if (user == null)
                throw new InvalidOperationException("Канал не найден");

            Subscription subscription = user.SubscribedTo
                .FirstOrDefault(u => u.SubscribedToId == userToId);

            if (subscription == null)
                return;

            user.SubscribedTo.Remove(subscription);

            user.SubscribedToCount--;
            userTo.SubscribersCount--;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsSubscribe(string  userId, string userToId )
        {
            return await _dbContext.Subscriptions
                .FirstOrDefaultAsync(s => s.SubscriberId == userId && s.SubscribedToId == userToId) == null;
        }
    }
}
