using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class SubscriptionService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public SubscriptionService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task SubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _unitOfWorks.ApplicationUsers.GetAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _unitOfWorks.ApplicationUsers.GetAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _unitOfWorks.Subscriptions.GetAsync(userId, userToId);
            if (subscription != null)
                return;

            subscription = new Subscription()
            {
                SubscriberId = userId,
                SubscribedToId = userToId
            };

            _unitOfWorks.Subscriptions.Add(subscription);

            user.SubscribedToCount++;
            userTo.SubscribersCount++;

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task UnSubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _unitOfWorks.ApplicationUsers.GetAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _unitOfWorks.ApplicationUsers.GetAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _unitOfWorks.Subscriptions.GetAsync(userId, userToId);

            if (subscription == null)
                return;

            _unitOfWorks.Subscriptions.Remove(subscription);

            user.SubscribedToCount--;
            userTo.SubscribersCount--;

            await _unitOfWorks.SaveChangesAsync();
        }

        public async Task<bool> IsSubscribeAsync(string userId, string userToId)
        {
            return await _unitOfWorks.Subscriptions
                .ContainsAsync(userId, userToId);
        }
    }
}
