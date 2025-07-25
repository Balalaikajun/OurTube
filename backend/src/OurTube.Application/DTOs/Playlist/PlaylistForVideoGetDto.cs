namespace OurTube.Application.DTOs.Playlist;

public class PlaylistForVideoGetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public uint Count { get; set; }
    public bool HasVideo { get; set; }
}