namespace OurTube.Application.Requests.Views;

/// <summary>
/// Запрос на регистрацию просмотра
/// </summary>
public class PostViewsRequest
{
    /// <summary>
    /// Идентификатор видео
    /// </summary>
    public Guid VideoId { get; set; }
    
    /// <summary>
    /// Время просмотра видео
    /// </summary>
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
}