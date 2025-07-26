namespace OurTube.Application.Replies.Playlist;

/// <summary>
/// Модель плейлиста, связанного с видео.
/// </summary>
public class PlaylistForVideo
{
    /// <summary>
    /// Уникальный идентификатор плейлиста.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название плейлиста.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Количество видео в плейлисте (неотрицательное значение).
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Признак того, что указанное видео находится в этом плейлисте.
    /// </summary>
    public bool HasVideo { get; set; }
}