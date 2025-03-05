using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.DTOs.Playlist
{
    public class PlaylistPatchDTO
    {
        
            [MaxLength(150)]
            [Required]
            public string? Title { get; set; }
            [MaxLength(5000)]
            [Required]
            public string? Description { get; set; }
        
    }
}
