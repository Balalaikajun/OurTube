using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public class VideoPlaylist : IBlob
{
    public int VideoId { get; set; }
    public int Resolution { get; set; }

    //Navigation
    public Video Video { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
}