using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public class VideoPlaylist : Base, IBlob
{
    public Guid VideoId { get; set; }
    public int Resolution { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
    
    //Navigation
    public Video Video { get; set; }
}