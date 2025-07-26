using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

namespace OurTube.Api.Controllers;

/// <summary>
/// Контроллер для поиска видео.
/// </summary>
[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Поиск видео по запросу пользователя.
    /// </summary>
    /// <param name="query">Строка поискового запроса (по умолчанию пустая).</param>
    /// <param name="limit">Максимальное количество видео в ответе (по умолчанию 10).</param>
    /// <param name="after">Смещение для пагинации (по умолчанию 0).</param>
    /// <param name="reload">Флаг перезагрузки результатов поиска (по умолчанию true).</param>
    /// <returns>Список видео, соответствующих запросу.</returns>
    /// <response code="200">Видео успешно найдены и возвращены.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] string query = "",
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = true)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _searchService.SearchVideos(new SearchRequest()
        {
            SearchQuery = query,
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });

        return Ok(result);
    }
}
