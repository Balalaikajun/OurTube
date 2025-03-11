using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class SubscriptionService
    {
        private IUnitOfWorks _unitOfWorks;

        public SubscriptionService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task Subscribe(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            ApplicationUser user = _unitOfWorks.ApplicationUsers.Get(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            ApplicationUser userTo = _unitOfWorks.ApplicationUsers.Get(userToId);

            if (user == null)
                throw new InvalidOperationException("Канал не найден");

            Subscription subscription = _unitOfWorks.Subscriptions.Get(userId, userToId);
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

        public async Task UnSubscribe(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            ApplicationUser user = _unitOfWorks.ApplicationUsers.Get(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            ApplicationUser userTo = _unitOfWorks.ApplicationUsers.Get(userToId);

            if (user == null)
                throw new InvalidOperationException("Канал не найден");

            Subscription subscription = _unitOfWorks.Subscriptions.Get(userId, userToId);

            if (subscription == null)
                return;

            _unitOfWorks.Subscriptions.Remove(subscription);

            user.SubscribedToCount--;
            userTo.SubscribersCount--;

            await _unitOfWorks.SaveChangesAsync();
        }

        public bool IsSubscribe(string userId, string userToId)
        {
            return _unitOfWorks.Subscriptions
                .Contains(userId, userToId);
        }
    }
}
