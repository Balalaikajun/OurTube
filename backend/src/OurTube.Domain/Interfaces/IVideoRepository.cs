using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IVideoRepository : IRepository<Video>
    {
        Video GetFullVideoData(int videoId);
        Video GetMinVideoData(int videoId);

    }
}
