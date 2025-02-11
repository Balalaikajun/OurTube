using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(Resolution))]
    public class VideoFile
    {
        public int VideoId { get; set; }
        public int Resolution { get; set; }
        [Required]
        [MaxLength(125)]
        public string VideoPath { get; set; }

        //Navigation
        public Video Video { get; set; }

    }
}
