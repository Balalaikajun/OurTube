using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Recommendation;

namespace OurTube.Api.Controllers;

/// <summary>
/// Контроллер рекомендаций видео для пользователя.
/// </summary>
[Route("[controller]")]
[ApiController]
public class RecommendationController : ControllerBase
{
    private readonly IRecomendationService _recommendationService;

    public RecommendationController(IRecomendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    /// <summary>
    /// Получить список рекомендованных видео для текущего пользователя.
    /// </summary>
    /// <param name="limit">Максимальное количество видео для загрузки (по умолчанию 10).</param>
    /// <param name="after">Смещение для пагинации (по умолчанию 0).</param>
    /// <param name="reload">Флаг перезагрузки рекомендаций (по умолчанию false).</param>
    /// <returns>Список рекомендуемых видео с пагинацией.</returns>
    /// <response code="200">Список рекомендаций успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _recommendationService.GetRecommendationsAsync(new GetRecommendationsRequest()
        {
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });

        return Ok(result);
    }

    /// <summary>
    /// Получить рекомендации видео, основанные на конкретном видео.
    /// </summary>
    /// <param name="videoId">Идентификатор видео для рекомендаций.</param>
    /// <param name="limit">Максимальное количество видео для загрузки (по умолчанию 10).</param>
    /// <param name="after">Смещение для пагинации (по умолчанию 0).</param>
    /// <param name="reload">Флаг перезагрузки рекомендаций (по умолчанию false).</param>
    /// <returns>Список рекомендуемых видео, связанных с указанным видео.</returns>
    /// <response code="200">Список рекомендаций успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet("video/{videoId:guid}")]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(OurTube.Application.Replies.Common.Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> GetForVideo(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _recommendationService.GetRecommendationsForVideoAsync(new GetRecommendationsForVideoRequest()
        {
            VideoId = videoId,
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });

        return Ok(result);
    }
}
