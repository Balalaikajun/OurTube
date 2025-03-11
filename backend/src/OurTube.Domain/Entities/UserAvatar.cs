using OurTube.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OurTube.Domain.Entities
{

    public class UserAvatar : IBlob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string UserId { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileName { get; set; }
        [Required]
        [MaxLength(125)]
        public string FileDirInStorage { get; set; }
        [MaxLength(25)]
        [Required]
        public string Bucket { get; set; }

        // Navigation
        public ApplicationUser User { get; set; }
    }
}
