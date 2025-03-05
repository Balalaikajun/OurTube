using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Playlist
{
    public class PlaylistMinGetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Count { get; set; }
    }
}
