using OurTube.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class VideoPreview : IBlob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileName { get; set; }
        [MaxLength(25)]
        [Required]
        public string Bucket { get; set; }

        // Navigation
        public Video Video { get; set; }
    }

}
