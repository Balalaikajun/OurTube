using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class SubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _unitOfWork.ApplicationUsers.GetAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _unitOfWork.ApplicationUsers.GetAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _unitOfWork.Subscriptions.GetAsync(userId, userToId);
            if (subscription != null)
                return;

            subscription = new Subscription()
            {
                SubscriberId = userId,
                SubscribedToId = userToId
            };

            _unitOfWork.Subscriptions.Add(subscription);

            user.SubscribedToCount++;
            userTo.SubscribersCount++;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnSubscribeAsync(string userId, string userToId)
        {
            if (userId == userToId)
                throw new InvalidOperationException("Id подписчика и канала совпадают");

            var user =await _unitOfWork.ApplicationUsers.GetAsync(userId);

            if (user == null)
                throw new InvalidOperationException("Пользователь не найден");

            var userTo =await _unitOfWork.ApplicationUsers.GetAsync(userToId);

            if (userTo == null)
                throw new InvalidOperationException("Канал не найден");

            var subscription =await _unitOfWork.Subscriptions.GetAsync(userId, userToId);

            if (subscription == null)
                return;

            _unitOfWork.Subscriptions.Remove(subscription);

            user.SubscribedToCount--;
            userTo.SubscribersCount--;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsSubscribeAsync(string userId, string userToId)
        {
            return await _unitOfWork.Subscriptions
                .ContainsAsync(userId, userToId);
        }
    }
}
