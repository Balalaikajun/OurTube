using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class SubscriptionService
    {
        private readonly IApplicationDbContext _dbContext;

        public SubscriptionService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _dbContext.ApplicationUsers.FindAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _dbContext.ApplicationUsers.FindAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _dbContext.Subscriptions.FindAsync(userId, userToId);
            if (subscription != null)
                return;

            subscription = new Subscription()
            {
                SubscriberId = userId,
                SubscribedToId = userToId
            };

            _dbContext.Subscriptions.Add(subscription);

            user.SubscribedToCount++;
            userTo.SubscribersCount++;

            await _dbContext.SaveChangesAsync();
        }

        public async Task UnSubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _dbContext.ApplicationUsers.FindAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _dbContext.ApplicationUsers.FindAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _dbContext.Subscriptions.FindAsync(userId, userToId);

            if (subscription == null)
                return;

            _dbContext.Subscriptions.Remove(subscription);

            user.SubscribedToCount--;
            userTo.SubscribersCount--;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsSubscribeAsync(string userId, string userToId)
        {
            return await _dbContext.Subscriptions
                .AnyAsync(s => s.SubscribedToId==userId && s.SubscriberId==userToId);
        }
    }
}
