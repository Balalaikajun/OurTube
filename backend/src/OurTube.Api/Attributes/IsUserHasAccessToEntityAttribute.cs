namespace OurTube.Api.Attributes;

/// <summary>
/// Проверка прав пользователя на доступ к сущности
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class IsUserHasAccessToEntityAttribute(Type entityType) : Attribute
{
    /// <summary>
    /// Тип сущности
    /// </summary>
    public Type EntityType { get; set; } = entityType;

    /// <summary>
    /// QueryString-параметр метода с идентификатором сущности
    /// </summary>
    public string? FromQuery { get; init; }

    /// <summary>
    /// Параметр в URL метода с идентификатором сущности
    /// </summary>
    public string? FromRoute { get; init; }

    /// <summary>
    /// Параметр в данных формы с идентификатором сущности
    /// </summary>
    public string? FromForm { get; init; }

    /// <summary>
    /// Параметр в теле запроса с идентификатором сущности
    /// </summary>
    public string? FromBody { get; init; }
}