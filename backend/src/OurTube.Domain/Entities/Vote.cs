using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    [PrimaryKey(nameof(VideoId), nameof(ApplicationUserId))]
    public class Vote
    {
        public int VideoId { get; set; }
        public string ApplicationUserId { get; set; }
        [Required]
        public bool Type { get; set; }
        [Required]
        public DateTime Created { get; set; }

        //Navigation
        public Video Video { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
