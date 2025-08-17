using System.ComponentModel;

namespace OurTube.Application.Requests.Common;

/// <summary>
///     Запрос для получения рекомендаций.
/// </summary>
public class PaginaionQueryParametersWithReload:BasePaginationQueryParameters
{
    /// <summary>
    ///     Флаг, указывающий, нужно ли перезагрузить комментарии полностью, игнорируя кэш.
    /// </summary>
    [DefaultValue(false)]
    public bool Reload { get; set; } =  false;
}