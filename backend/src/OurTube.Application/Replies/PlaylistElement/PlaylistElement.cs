using OurTube.Application.Replies.Video;

namespace OurTube.Application.Replies.PlaylistElement;

/// <summary>
/// Модель для элемента плейлиста, содержащий информацию о видео и дате добавления.
/// </summary>
public class PlaylistElement
{
    /// <summary>
    /// Дата и время добавления видео в плейлист (в формате UTC).
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Минимальная информация о видео.
    /// </summary>
    public required MinVideo Video { get; set; }
}