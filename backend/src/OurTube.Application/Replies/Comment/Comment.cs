namespace OurTube.Application.Replies.Comment;

/// <summary>
///     Представляет комментарий к какому-либо объекту (например, видео или посту).
/// </summary>
public class Comment
{
    /// <summary>
    ///     Уникальный идентификатор комментария.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Текстовое содержимое комментария.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Дата и время создания комментария (в формате UTC).
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    ///     Дата и время последнего редактирования комментария (в формате UTC).
    ///     Может быть <see langword="null" />, если комментарий не редактировался.
    /// </summary>
    public DateTime? UpdatedDate { get; set; }

    /// <summary>
    ///     Дата и время удаления комментария (в формате UTC).
    ///     Может быть <see langword="null" />, если комментарий не был удалён.
    /// </summary>
    public DateTime? DeletedDate { get; set; }

    /// <summary>
    ///     Идентификатор родительского комментария, если этот комментарий является ответом.
    ///     Может быть <see langword="null" />, если комментарий является корневым.
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    ///     Флаг, указывающий, редактировался ли комментарий после создания.
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    ///     Флаг, указывающий, был ли комментарий удалён.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    ///     Голос пользователя за комментарий: <c>true</c> — лайк, <c>false</c> — дизлайк,
    ///     <see langword="null" /> — пользователь не голосовал.
    /// </summary>
    public bool? Vote { get; set; }

    /// <summary>
    ///     Количество лайков комментария.
    /// </summary>
    public int LikesCount { get; set; }

    /// <summary>
    ///     Количество дочерних (вложенных) комментариев.
    /// </summary>
    public int ChildsCount { get; set; }

    /// <summary>
    ///     Количество дизлайков комментария.
    /// </summary>
    public int DislikesCount { get; set; }

    /// <summary>
    ///     Пользователь, оставивший комментарий.
    /// </summary>
    public required ApplicationUser.ApplicationUser User { get; set; }
}