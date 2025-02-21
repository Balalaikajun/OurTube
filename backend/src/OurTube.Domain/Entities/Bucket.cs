using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Domain.Entities
{
    public class Bucket
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(25)]
        [Required]
        public string Name { get; set; }
    }
}
