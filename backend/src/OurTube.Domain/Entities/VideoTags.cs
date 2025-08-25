namespace OurTube.Domain.Entities;

public class VideoTags : Base
{
    public Guid TagId { get; set; }
    public Guid VideoId { get; set; }
    
    public Tag Tag { get; }
    public Video Video { get; }
}