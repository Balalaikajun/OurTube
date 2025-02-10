using System.ComponentModel.DataAnnotations;

namespace backend.src.OurTube.Domain.Entities
{
    public class VideoFile
    {
        [Key]
        public int VideoId { get; set; }
        [Key]   
        public int Resolution { get; set; }
        [Required]
        [MaxLength(125)]
        public string VideoPath{ get; set; }

        //Navigation
        public Video Video { get; set; }

    }
}
