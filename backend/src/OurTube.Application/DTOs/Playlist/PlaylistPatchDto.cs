using System.ComponentModel.DataAnnotations;

namespace OurTube.Application.DTOs.Playlist;

public class PlaylistPatchDto
{
    [MaxLength(150)] [Required] public string? Title { get; set; }

    [MaxLength(5000)] [Required] public string? Description { get; set; }
}