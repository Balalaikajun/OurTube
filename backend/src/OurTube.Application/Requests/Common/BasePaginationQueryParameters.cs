using System.ComponentModel;

namespace OurTube.Application.Requests.Common;

/// <summary>
///     Базовый запрос для получения элементов с параметрами пагинации.
/// </summary>
public class BasePaginationQueryParameters
{
    /// <summary>
    ///     Максимальное количество комментариев, возвращаемых за один запрос.
    /// </summary>
    [DefaultValue(10)]
    public int Limit { get; set; } = 10;

    /// <summary>
    ///     Смещение для пагинации — номер комментария, после которого следует вернуть следующие.
    /// </summary>
    [DefaultValue(0)]
    public int After { get; set; } = 0;
}