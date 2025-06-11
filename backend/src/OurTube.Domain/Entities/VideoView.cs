namespace OurTube.Domain.Entities;

public class VideoView
{
    public int VideoId { get; set; }
    public string ApplicationUserId { get; set; }
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
    public TimeSpan? WhatchTime { get; set; }
    public DateTime DateTime { get; set; } = DateTime.UtcNow;

    //Navigation
    public Video Video { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}