using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(ApplicationUserId))]
    public class VideoView
    {
        public int VideoId { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
        public TimeSpan? WhatchTime { get; set; }
        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        //Navigation
        public Video Video { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
