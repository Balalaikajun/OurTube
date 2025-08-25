namespace OurTube.Application.Interfaces;

public interface ISubscriptionService
{
    Task SubscribeAsync(Guid userId, Guid userToId);
    Task UnSubscribeAsync(Guid userId, Guid userToId);
    Task<bool> IsSubscribeAsync(Guid userId, Guid userToId);
}