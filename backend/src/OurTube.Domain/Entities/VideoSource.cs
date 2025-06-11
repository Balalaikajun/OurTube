using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public class VideoSource : IBlob
{
    public int VideoId { get; set; }

    // Navigation
    public Video Video { get; set; }
    public string FileName { get; set; }
    public string Bucket { get; set; }
}