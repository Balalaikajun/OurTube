using OurTube.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    public class VideoSource : IBlob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileName { get; set; }
        [Required]
        public int BucketId { get; set; }

        // Navigation
        public Video Video { get; set; }
        public Bucket Bucket { get; set; }
    }
}
