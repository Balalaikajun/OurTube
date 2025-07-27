namespace OurTube.Application.Replies.ApplicationUser;

/// <summary>
///     Модель данных пользователя, возвращаемая в ответах API.
/// </summary>
public class ApplicationUser
{
    /// <summary>
    ///     Уникальный идентификатор пользователя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Имя пользователя (отображаемое имя или логин).
    /// </summary>
    public required string UserName { get; set; }

    /// <summary>
    ///     Флаг, указывающий, подписан ли текущий автор запроса на данного пользователя.
    /// </summary>
    public bool IsSubscribed { get; set; }

    /// <summary>
    ///     Количество подписчиков у пользователя.
    /// </summary>
    public int SubscribersCount { get; set; }

    /// <summary>
    ///     Данные об аватаре пользователя. Может быть <see langword="null" />, если аватар не установлен.
    /// </summary>
    public UserAvatar.UserAvatar? UserAvatar { get; set; }
}