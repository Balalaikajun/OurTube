using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IVideoRepository : IRepository<Video>
    {
        Task<Video?> GetFullVideoDataAsync(int videoId);
        Task<Video?> GetMinVideoDataAsync(int videoId);

    }
}
