using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;

namespace OurTube.Api.Controllers;

/// <summary>
///     Контроллер рекомендаций видео для пользователя.
/// </summary>
[Route("[controller]")]
[ApiController]
public class RecommendationsController : ControllerBase
{
    private readonly IRecomendationService _recommendationService;

    public RecommendationsController(IRecomendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    /// <summary>
    ///     Получить список рекомендованных видео для текущего пользователя.
    /// </summary>
    /// <param name="parameters">Параметры пагинации и фильтрации.</param>
    /// <returns>Список рекомендуемых видео с пагинацией.</returns>
    /// <response code="200">Список рекомендаций успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] GetQuaryParameters parameters)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _recommendationService.GetRecommendationsAsync(
            userId, sessionId, parameters);

        return Ok(result);
    }

    /// <summary>
    ///     Получить рекомендации видео, основанные на конкретном видео.
    /// </summary>
    /// <param name="videoId">Идентификатор видео для рекомендаций.</param>
    /// <param name="parameters">Параметры пагинации и фильтрации.</param>
    /// <returns>Список рекомендуемых видео, связанных с указанным видео.</returns>
    /// <response code="200">Список рекомендаций успешно получен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet("videos/{videoId:guid}")]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> GetForVideo(
        Guid videoId,
        [FromQuery] GetQuaryParameters parameters)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _recommendationService.GetRecommendationsForVideoAsync(
            videoId, userId, sessionId, parameters);

        return Ok(result);
    }
}