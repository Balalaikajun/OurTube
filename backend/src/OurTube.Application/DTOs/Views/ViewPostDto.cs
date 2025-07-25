namespace OurTube.Application.DTOs.Views;

public class ViewPostDto
{
    public Guid VideoId { get; set; }
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
}