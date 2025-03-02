using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{
    public class Playlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public ICollection<PlaylistElement> Videos { get; set; }
    }
}
