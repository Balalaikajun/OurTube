namespace OurTube.Application.Requests.Comment;

/// <summary>
/// Запрос на добавление или обновление комментария к видео.
/// </summary>
public class UpdateCommentRequest
{
    /// <summary>
    /// Идентификатор комментария для обновления.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Идентификатор родительского комментария, если комментарий является ответом. 
    /// Может быть <see langword="null"/>, если комментарий верхнего уровня.
    /// </summary>
    public Guid? ParentId { get; set; } = null;
}