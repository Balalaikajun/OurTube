using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Playlist
{
    public class PlaylistGetDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<PlaylistElementGetDTO> PlaylistElements { get; set; }

    }
}
