using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(ApplicationUserId))]
    public class View
    {
        public int VideoId { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public long EndTime { get; set; }
        [Required]
        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        //Navigation
        public Video Video { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
