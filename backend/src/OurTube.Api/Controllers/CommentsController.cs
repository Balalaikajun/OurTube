using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Comment;
using OurTube.Application.Replies.Common;
using OurTube.Application.Requests.Comment;

namespace OurTube.Api.Controllers;

/// <summary>
///     Управление комментариями к видео.
/// </summary>
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentCrudService _commentCrudService;
    private readonly ICommentRecommendationService _commentRecommendationService;

    public CommentsController(
        ICommentCrudService commentCrudService,
        ICommentRecommendationService commentRecommendationService)
    {
        _commentCrudService = commentCrudService;
        _commentRecommendationService = commentRecommendationService;
    }

    /// <summary>
    ///     Добавить комментарий к видео.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <param name="request">Данные комментария.</param>
    /// <returns>Созданный комментарий.</returns>
    /// <response code="201">Комментарий успешно создан.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpPost("videos/{videoId:guid}/[Controller]")]
    [ProducesResponseType(typeof(Comment), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Comment>> Post(
        [FromRoute] Guid videoId,
        [FromBody] PostCommentRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _commentCrudService.CreateAsync(userId, videoId, request);

        return Created(string.Empty, result);
    }

    /// <summary>
    ///     Обновить комментарий.
    /// </summary>
    /// <param name="id">Идентификатор комментария для редактирования.</param>
    /// <param name="request">Данные для обновления комментария.</param>
    /// <response code="204">Комментарий успешно обновлён.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Domain.Entities.Comment), FromRoute = nameof(id))]
    [HttpPatch("[Controller]/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Patch(
        [FromRoute] Guid id,
        [FromBody] UpdateCommentRequest request)
    {
        await _commentCrudService.UpdateAsync(id, request);

        return NoContent();
    }

    /// <summary>
    ///     Удалить комментарий.
    /// </summary>
    /// <param name="id">Идентификатор комментария.</param>
    /// <response code="204">Комментарий успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Domain.Entities.Comment), FromRoute = nameof(id))]
    [HttpDelete("[controller]/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        await _commentCrudService.DeleteAsync(id);

        return NoContent();
    }

    /// <summary>
    ///     Получить список комментариев к видео с пагинацией и рекомендациями.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <param name="parameters">Параметры запроса.</param>
    /// <returns>Список комментариев с учётом ограничений.</returns>
    /// <response code="200">Комментарии успешно получены.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [HttpGet("videos/{videoId:guid}/[controller]")]
    [ProducesResponseType(typeof(ListReply<Comment>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<Comment>>> GetWithLimit(
        [FromRoute] Guid videoId,
        [FromQuery] GetCommentQueryParameters parameters)
    {
        var nameId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (Guid.TryParse(nameId, out var guid))
        {
            userId = guid;
        }
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result =
            await _commentRecommendationService.GetCommentsWithLimitAsync(videoId, userId, sessionId, parameters);

        return Ok(result);
    }
}