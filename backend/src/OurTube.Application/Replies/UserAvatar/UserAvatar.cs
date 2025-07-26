namespace OurTube.Application.Replies.UserAvatar;

/// <summary>
/// Модель, содержащий информацию об аватаре пользователя.
/// </summary>
public class UserAvatar
{
    /// <summary>
    /// Уникальный идентификатор аватара.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя, которому принадлежит аватар.
    /// </summary>
    public required string UserId { get; set; }

    /// <summary>
    /// Имя файла аватара.
    /// </summary>
    public required string FileName { get; set; }

    /// <summary>
    /// Имя хранилища (bucket), где хранится аватар.
    /// </summary>
    public required string Bucket { get; set; }
}