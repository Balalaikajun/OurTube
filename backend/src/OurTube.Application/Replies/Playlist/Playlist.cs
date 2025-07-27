namespace OurTube.Application.Replies.Playlist;

/// <summary>
///     Модель плейлиста с описанием и списком элементов.
/// </summary>
public class Playlist
{
    /// <summary>
    ///     Идентификатор плейлиста.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Название плейлиста.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    ///     Количество элементов (видео) в плейлисте.
    /// </summary>
    public int Count { get; set; }
}