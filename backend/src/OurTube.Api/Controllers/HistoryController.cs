using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Common;
using OurTube.Application.Requests.Views;

namespace OurTube.Api.Controllers;

/// <summary>
///     Управление историей просмотров пользователя.
/// </summary>
[Route("users/me/watch-history")]
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly IViewService _viewService;

    public HistoryController(IViewService viewService)
    {
        _viewService = viewService;
    }

    /// <summary>
    ///     Добавить видео в историю просмотров.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <param name="postDto">Запрос с информацией о просмотренном видео.</param>
    /// <response code="204">Видео успешно добавлено в историю.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpPost("videos/{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AddVideo(
        [FromRoute] Guid videoId,
        [FromBody] PostViewsRequest postDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.AddVideoAsync(userId, videoId, postDto);

        return NoContent();
    }

    /// <summary>
    ///     Удалить видео из истории просмотров.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <response code="204">Видео успешно удалено из истории.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Видео не найдено в истории.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpDelete("videos/{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveVideo(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.RemoveVideoAsync(videoId, userId);

        return NoContent();
    }

    /// <summary>
    ///     Очистить всю историю просмотров пользователя.
    /// </summary>
    /// <response code="204">История просмотров успешно очищена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ClearHistory()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.ClearHistoryAsync(userId);

        return NoContent();
    }

    /// <summary>
    ///     Получить историю просмотров пользователя с пагинацией и фильтрацией.
    /// </summary>
    /// <returns>Список просмотренных видео с ограничениями.</returns>
    /// <response code="200">История просмотров успешно получена.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ListReply<MinVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] GetQueryParametersWithSearch parameters)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _viewService.GetWithLimitAsync(userId, parameters);

        return Ok(result);
    }
}