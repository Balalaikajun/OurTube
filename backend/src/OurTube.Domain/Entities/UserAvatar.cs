using Microsoft.EntityFrameworkCore;
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
    
    public class UserAvatar: IBlob
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
        [Required]
        public int BucketId {  get; set; }

        // Navigation
        public ApplicationUser User { get; set; }
        public Bucket Bucket { get; set; }
    }
}
