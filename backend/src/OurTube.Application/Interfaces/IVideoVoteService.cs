namespace OurTube.Application.Interfaces;

public interface IVideoVoteService
{
    Task SetAsync(Guid videoId, Guid userId, bool type);
    Task DeleteAsync(Guid videoId, Guid userId);
}