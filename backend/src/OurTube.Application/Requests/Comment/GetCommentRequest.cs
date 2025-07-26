namespace OurTube.Application.Requests.Comment;

/// <summary>
/// Запрос для получения комментариев к видео с параметрами пагинации и фильтрации.
/// </summary>
public class GetCommentRequest
{
    /// <summary>
    /// Идентификатор видео, к которому запрашиваются комментарии.
    /// </summary>
    public Guid VideoId { get; set; }

    /// <summary>
    /// Максимальное количество комментариев, возвращаемых за один запрос.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Смещение для пагинации — номер комментария, после которого следует вернуть следующие.
    /// </summary>
    public int After { get; set; }

    /// <summary>
    /// Идентификатор сессии пользователя, делающего запрос.
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Идентификатор пользователя, для которого делается запрос. Может быть <see langword="null"/>, если пользователь не авторизован.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Идентификатор родительского комментария для получения вложенных ответов. Может быть <see langword="null"/>.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Флаг, указывающий, нужно ли перезагрузить комментарии полностью, игнорируя кэш.
    /// </summary>
    public bool Reload { get; set; }
}