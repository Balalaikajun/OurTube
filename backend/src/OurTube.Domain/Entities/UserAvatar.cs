using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public class UserAvatar : IBlob
{
    public string UserId { get; set; }

    public string FileDirInStorage { get; set; }

    // Navigation
    public ApplicationUser ApplicationUser { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
}