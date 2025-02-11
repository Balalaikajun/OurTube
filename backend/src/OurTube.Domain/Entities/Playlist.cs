using System.ComponentModel.DataAnnotations;

namespace OurTube.Domain.Entities
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(150)]
        [Required]
        public string Title { get; set; }
        [MaxLength(5000)]
        [Required]
        public string Description { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }

        //Navigation
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Video> Videos { get; set; }
    }
}
