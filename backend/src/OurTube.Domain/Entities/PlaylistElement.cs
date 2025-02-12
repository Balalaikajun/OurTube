using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(PlaylistId),nameof(VideoId))]
    public class PlaylistElement
    {
        public int PlaylistId { get; set; }
        public int VideoId { get; set; }

        //Navigation
        public Playlist Playlist { get; set; }
        public Video Video { get; set; }
    }
}
