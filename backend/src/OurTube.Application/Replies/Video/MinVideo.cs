namespace OurTube.Application.Replies.Video;

/// <summary>
/// Минимальная информация о видео для отображения в списках и превью.
/// </summary>
public class MinVideo
{
    /// <summary>
    /// Уникальный идентификатор видео.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название видео.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Количество просмотров видео.
    /// </summary>
    public int ViewsCount { get; set; }

    /// <summary>
    /// Голос пользователя за видео: <c>true</c> — лайк, <c>false</c> — дизлайк, 
    /// <see langword="null"/> — пользователь не голосовал.
    /// </summary>
    public bool? Vote { get; set; }

    /// <summary>
    /// Продолжительность видео.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Время окончания просмотра видео (если есть), например для отметки прогресса.
    /// Может быть <see langword="null"/>.
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Дата и время создания видео (в формате UTC).
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Превью видео.
    /// </summary>
    public required VideoPreview.VideoPreview Preview { get; set; }

    /// <summary>
    /// Пользователь, загрузивший видео.
    /// </summary>
    public required ApplicationUser.ApplicationUser User { get; set; }
}