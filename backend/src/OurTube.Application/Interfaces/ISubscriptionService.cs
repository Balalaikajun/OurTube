namespace OurTube.Application.Interfaces;

public interface ISubscriptionService
{
    Task SubscribeAsync(string userId, string userToId);
    Task UnSubscribeAsync(string userId, string userToId);
    Task<bool> IsSubscribeAsync(string userId, string userToId);
}