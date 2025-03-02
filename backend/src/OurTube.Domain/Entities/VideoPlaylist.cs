using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(Resolution))]
    public class VideoPlaylist: IBlob
    {
        public int VideoId { get; set; }
        public int Resolution { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileName { get; set; }
        [Required]
        public int BucketId { get; set; }

        //Navigation
        public Video Video { get; set; }
        public Bucket Bucket { get; set; }

    }
}
