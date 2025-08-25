namespace OurTube.Domain.Entities;

public class VideoView : Base
{
    public Guid VideoId { get; set; }
    public Guid ApplicationUserId { get; set; }
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
    public TimeSpan? WhatchTime { get; set; }
    
    //Navigation
    public Video Video { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}