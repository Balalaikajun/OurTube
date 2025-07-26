namespace OurTube.Application.Replies.Video;

/// <summary>
/// Модель видео с полной информацией.
/// </summary>
public class Video
{
    /// <summary>
    /// Уникальный идентификатор видео.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок видео.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Описание видео.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Количество лайков.
    /// </summary>
    public int LikesCount { get; set; }

    /// <summary>
    /// Количество дизлайков.
    /// </summary>
    public int DislikesCount { get; set; }

    /// <summary>
    /// Количество комментариев под видео.
    /// </summary>
    public int CommentsCount { get; set; }

    /// <summary>
    /// Общее количество просмотров видео.
    /// </summary>
    public int ViewsCount { get; set; }

    /// <summary>
    /// Голос текущего пользователя по видео: <c>true</c> — лайк, <c>false</c> — дизлайк,
    /// <see langword="null"/> — пользователь не голосовал.
    /// </summary>
    public bool? Vote { get; set; }

    /// <summary>
    /// Продолжительность видео.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Время окончания просмотра видео текущим пользователем (если есть).
    /// Может быть <see langword="null"/>.
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Дата и время создания видео (UTC).
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Превью видео.
    /// </summary>
    public required VideoPreview.VideoPreview Preview { get; set; }

    /// <summary>
    /// Список файлов видео (различные форматы, качества).
    /// </summary>
    public required List<VideoPlaylist.VideoPlaylist> Files { get; set; }

    /// <summary>
    /// Информация о пользователе, загрузившем видео.
    /// </summary>
    public required ApplicationUser.ApplicationUser User { get; set; }
}