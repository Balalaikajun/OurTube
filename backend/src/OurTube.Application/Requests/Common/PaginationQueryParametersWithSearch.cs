using System.ComponentModel;

namespace OurTube.Application.Requests.Common;

/// <summary>
///     Запрос для поиска видео по строке запроса.
/// </summary>
public class PaginationQueryParametersWithSearch : BasePaginationQueryParameters
{
    /// <summary>
    ///     Строка поиска по названиям
    /// </summary>
    public string? Query { get; set; } = null;
}