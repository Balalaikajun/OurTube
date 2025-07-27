namespace OurTube.Domain.Entities;

public class ApplicationUser : Base
{
    public string UserName { get; set; }
    public int SubscribersCount { get; set; } = 0;
    public int SubscribedToCount { get; set; } = 0;
    
    //Navigation
    public ICollection<Subscription> Subscribers { get; set; }
    public ICollection<Subscription> SubscribedTo { get; set; }
    public ICollection<Playlist> Playlists { get; set; }
    public UserAvatar? UserAvatar { get; set; }
}