namespace OurTube.Application.Interfaces;

public interface IVideoVoteService
{
    Task SetAsync(int videoId, string userId, bool type);
    Task DeleteAsync(int videoId, string userId);
}