namespace OurTube.Application.Requests.Playlist;

/// <summary>
/// Запрос на добавление видео в плейлист
/// </summary>
public class AddVideoRequest
{
    /// <summary>
    /// Идентификатор видео 
    /// </summary>
    public  Guid VideoId { get; set; }
}