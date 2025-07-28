namespace OurTube.Application.Requests.Comment;

/// <summary>
///     Запрос на добавление или обновление комментария к видео.
/// </summary>
public class UpdateCommentRequest
{
    /// <summary>
    ///     Текст комментария.
    /// </summary>
    public string Text { get; set; }
}