namespace OurTube.Application.Interfaces;

/// <summary>
///     Сервис для проверки доступа
/// </summary>
public interface IAccessChecker
{
    /// <summary>
    ///     Метод проверки прав на редактирование у пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="entityId">Идентификатор сущности</param>
    /// <typeparam name="TEntity">Сущность доступ к которой необходимо проверить</typeparam>
    Task<bool> CanEditAsync(Type type, Guid userId, Guid entityId);
}