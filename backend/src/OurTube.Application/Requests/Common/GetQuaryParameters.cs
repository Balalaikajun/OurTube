namespace OurTube.Application.Requests.Common;

/// <summary>
///     Базовый запрос для получения элементов с параметрами пагинации и фильтрации.
/// </summary>
public class GetQuaryParameters
{
    /// <summary>
    ///     Максимальное количество комментариев, возвращаемых за один запрос.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    ///     Смещение для пагинации — номер комментария, после которого следует вернуть следующие.
    /// </summary>
    public int After { get; set; }

    /// <summary>
    ///     Флаг, указывающий, нужно ли перезагрузить комментарии полностью, игнорируя кэш.
    /// </summary>
    public bool Reload { get; set; }
}