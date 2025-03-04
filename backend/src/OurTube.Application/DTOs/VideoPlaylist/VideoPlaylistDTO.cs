using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.VideoPlaylist
{
    public class VideoPlaylistDTO
    {
        public int Resolution { get; set; }
        public string FileName { get; set; }
        public string BucketName { get; set; }
    }
}
