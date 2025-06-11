namespace OurTube.Domain.Entities;

public class Playlist
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = "";
    public int Count { get; set; }
    public string ApplicationUserId { get; set; }
    public bool IsSystem { get; set; } = false;

    //Navigation
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<PlaylistElement> PlaylistElements { get; set; }
}