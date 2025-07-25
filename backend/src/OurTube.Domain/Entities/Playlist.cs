namespace OurTube.Domain.Entities;

public class Playlist : Base
{
    public string Title { get; set; }
    public string Description { get; set; } = "";
    public uint Count { get; set; }
    public Guid ApplicationUserId { get; set; }
    public bool IsSystem { get; set; } = false;
    
    //Navigation
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<PlaylistElement> PlaylistElements { get; set; }
}