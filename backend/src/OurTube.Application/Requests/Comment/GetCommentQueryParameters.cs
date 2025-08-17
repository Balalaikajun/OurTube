using OurTube.Application.Requests.Common;

namespace OurTube.Application.Requests.Comment;

/// <summary>
///     Запрос для получения комментариев к видео с параметрами пагинации и фильтрации.
/// </summary>
public class GetCommentQueryParameters : PaginaionQueryParametersWithReload
{
    /// <summary>
    ///     Идентификатор родительского комментария для получения вложенных ответов. Может быть <see langword="null" />.
    /// </summary>
    public Guid? ParentId { get; set; }
}