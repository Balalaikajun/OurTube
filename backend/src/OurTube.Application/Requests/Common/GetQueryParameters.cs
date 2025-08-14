using System.ComponentModel;

namespace OurTube.Application.Requests.Common;

/// <summary>
///     Базовый запрос для получения элементов с параметрами пагинации и фильтрации.
/// </summary>
public class GetQueryParameters
{
    /// <summary>
    ///     Максимальное количество комментариев, возвращаемых за один запрос. (По умолчанию 10)
    /// </summary>
    [DefaultValue(10)]
    public int Limit { get; set; } = 10;

    /// <summary>
    ///     Смещение для пагинации — номер комментария, после которого следует вернуть следующие.
    /// </summary>
    [DefaultValue(0)]
    public int After { get; set; } = 0;

    /// <summary>
    ///     Флаг, указывающий, нужно ли перезагрузить комментарии полностью, игнорируя кэш.
    /// </summary>
    [DefaultValue(false)]
    public bool Reload { get; set; } =  false;
}