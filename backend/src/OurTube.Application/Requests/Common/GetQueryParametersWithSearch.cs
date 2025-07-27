namespace OurTube.Application.Requests.Common;

/// <summary>
///     Запрос для получения элементов со строкой поиска, параметрами пагинации и фильтрации.
/// </summary>
public class GetQueryParametersWithSearch : GetQuaryParameters
{
    /// <summary>
    ///     Строка поиска по названиям
    /// </summary>
    public string? SearchQuery { get; set; } = null;
}