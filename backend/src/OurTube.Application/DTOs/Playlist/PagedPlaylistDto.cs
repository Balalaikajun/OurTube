namespace OurTube.Application.DTOs.Playlist;

public class PagedPlaylistDto
{
    public PlaylistGetDto Playlist { get; set; }
    public int? NextAfter { get; set; }
}