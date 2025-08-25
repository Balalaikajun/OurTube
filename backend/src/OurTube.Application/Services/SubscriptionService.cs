using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _dbContext;

    public SubscriptionService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SubscribeAsync(Guid userId, Guid userToId)
    {
        if (userId == userToId)
            throw new InvalidOperationException("Id подписчика и канала совпадают");

        var user = await _dbContext.ApplicationUsers
            .GetByIdAsync(userId, true);

        var userTo = await _dbContext.ApplicationUsers
            .GetByIdAsync(userToId, true);

        await _dbContext.Subscriptions
            .EnsureExistAsync(s => s.SubscriberId == userToId && s.SubscribedToId == userToId);

        var subscription = new Subscription
        {
            SubscriberId = userId,
            SubscribedToId = userToId
        };

        _dbContext.Subscriptions.Add(subscription);

        user.SubscribedToCount++;
        userTo.SubscribersCount++;

        await _dbContext.SaveChangesAsync();
    }

    public async Task UnSubscribeAsync(Guid userId, Guid userToId)
    {
        if (userId == userToId)
            throw new InvalidOperationException("Id подписчика и канала совпадают");

        var user = await _dbContext.ApplicationUsers
            .GetByIdAsync(userId, true);

        var userTo = await _dbContext.ApplicationUsers
            .GetByIdAsync(userToId, true);

        var subscription = await _dbContext.Subscriptions
            .GetAsync(s => s.SubscriberId == userId && s.SubscribedToId == userToId, true);

        subscription.Delete();

        user.SubscribedToCount--;
        userTo.SubscribersCount--;

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> IsSubscribeAsync(Guid userId, Guid userToId)
    {
        return await _dbContext.Subscriptions
            .AnyAsync(s => s.SubscribedToId == userId && s.SubscriberId == userToId);
    }
}