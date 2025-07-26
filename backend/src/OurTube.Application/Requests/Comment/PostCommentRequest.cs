namespace OurTube.Application.Requests.Comment;

/// <summary>
/// DTO для создания нового комментария к видео.
/// </summary>
public class PostCommentRequest
{
    /// <summary>
    /// Идентификатор видео, к которому оставляется комментарий.
    /// </summary>
    public Guid VideoId { get; set; }

    /// <summary>
    /// Текст комментария.
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Идентификатор родительского комментария, если комментарий является ответом.
    /// Может быть <see langword="null"/>, если комментарий верхнего уровня.
    /// </summary>
    public Guid? ParentId { get; set; } = null;
}