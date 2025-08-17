using System.ComponentModel;

namespace OurTube.Application.Requests.Common;

/// <summary>
///     Запрос для получения элементов со строкой поиска, параметрами пагинации и фильтрации.
/// </summary>
public class GetQueryParametersWithSearch
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
    
    /// <summary>
    ///     Строка поиска по названиям
    /// </summary>
    public string? Query { get; set; } = null;
}