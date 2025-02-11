using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Infrastructure.Data
{
    internal class ApplicationDbContext: DbContext
    {
        public DbSet<ApplicationUser> applicationUsers {get; set;}
        public DbSet<Video> Videos { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<>

    }
}
