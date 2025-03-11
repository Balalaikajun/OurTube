using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories
{
    public class PlaylistRepository : Repository<Playlist>, IPlaylistRepository
    {
        public ApplicationDbContext ApplicationDbContext
        {
            get { return Context as ApplicationDbContext; }
        }

        public PlaylistRepository(ApplicationDbContext context)
        : base(context) { }

        public async Task<Playlist> GetPlaylistWithElements(int playlistId, int limit, int after)
        {
            return await ApplicationDbContext.Playlists
            .Include(p => p.PlaylistElements
                .OrderBy(pe => pe.AddedAt)
                .OrderBy(pe => pe.AddedAt)
                .Skip(after)
                .Take(limit))
            .FirstOrDefaultAsync(x => x.Id == playlistId);
        }


    }
}
