namespace OurTube.Domain.Entities;

public class UserAvatar : Base
{
    public Guid UserId { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
    
    // Navigation
    public ApplicationUser ApplicationUser { get; set; }
}