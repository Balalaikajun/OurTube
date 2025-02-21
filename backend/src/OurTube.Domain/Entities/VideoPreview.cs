using OurTube.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    public class VideoPreview:IBlob
    {
        [Key]
        public int VideoId { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileName { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileDirInStorage { get; set; }
        [Required]
        public int BucketId { get; set; }

        // Navigation
        public Video Video { get; set; }
        public Bucket Bucket { get; set; }
    }
}
