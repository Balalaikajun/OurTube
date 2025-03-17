using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces
{
    public interface IPlaylistRepository : IRepository<Playlist>
    {
        Task<Playlist> GetPlaylistWithElementsAsync(int playlistId, int limit, int after);
    }
}
