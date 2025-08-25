namespace OurTube.Application.Requests.Views;

/// <summary>
///     Запрос на регистрацию просмотра
/// </summary>
public class PostViewsRequest
{
    /// <summary>
    ///     Время просмотра видео
    /// </summary>
    public TimeSpan EndTime { get; set; } = TimeSpan.Zero;
}