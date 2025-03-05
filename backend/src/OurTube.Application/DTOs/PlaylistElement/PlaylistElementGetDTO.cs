using OurTube.Application.DTOs.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.PlaylistElement
{
    public class PlaylistElementGetDTO
    {

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public VideoDTO Video { get; set; }
    }
}
