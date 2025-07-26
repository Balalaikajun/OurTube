using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Requests.Comment;
using OurTube.Domain.Entities;

namespace OurTube.Api.Controllers;

/// <summary>
/// Управление комментариями к видео.
/// </summary>
[Route("Video/[Controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentCrudService _commentCrudService;
    private readonly ICommentRecommendationService _commentRecommendationService;

    public CommentController(
        ICommentCrudService commentCrudService,
        ICommentRecommendationService commentRecommendationService)
    {
        _commentCrudService = commentCrudService;
        _commentRecommendationService = commentRecommendationService;
    }

    /// <summary>
    /// Добавить комментарий к видео.
    /// </summary>
    /// <param name="postDto">Данные комментария.</param>
    /// <returns>Созданный комментарий.</returns>
    /// <response code="201">Комментарий успешно создан.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Application.Replies.Comment.Comment), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Application.Replies.Comment.Comment>> Post(
        [FromBody] PostCommentRequest postDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _commentCrudService.CreateAsync(userId, postDto);

        return Created(string.Empty, result);
    }

    /// <summary>
    /// Обновить комментарий.
    /// </summary>
    /// <param name="postDto">Данные для обновления комментария.</param>
    /// <response code="204">Комментарий успешно обновлён.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Комментарий не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Comment), FromBody = nameof(postDto.Id))]
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Patch([FromBody] UpdateCommentRequest postDto)
    {
        await _commentCrudService.UpdateAsync(postDto);

        return NoContent();
    }

    /// <summary>
    /// Удалить комментарий.
    /// </summary>
    /// <param name="commentId">Идентификатор комментария.</param>
    /// <response code="204">Комментарий успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Комментарий не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Comment), FromRoute = nameof(commentId))]
    [HttpDelete("{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(Guid commentId)
    {
        await _commentCrudService.DeleteAsync(commentId);

        return NoContent();
    }

    /// <summary>
    /// Получить список комментариев к видео с пагинацией и рекомендациями.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <param name="limit">Количество комментариев для загрузки (по умолчанию 10).</param>
    /// <param name="after">Смещение по количеству комментариев (для пагинации).</param>
    /// <param name="parentId">Идентификатор родительского комментария (для вложенных комментариев).</param>
    /// <param name="reload">Принудительно обновить список.</param>
    /// <returns>Список комментариев с учётом ограничений.</returns>
    /// <response code="200">Комментарии успешно получены.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [HttpGet("{videoId:guid}")]
    [ProducesResponseType(typeof(ListReply<Application.Replies.Comment.Comment>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<Application.Replies.Comment.Comment>>> GetWithLimit(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] Guid? parentId = null,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _commentRecommendationService.GetCommentsWithLimitAsync(new GetCommentRequest
        {
            VideoId = videoId,
            Limit = limit,
            After = after,
            SessionId = sessionId,
            UserId = userId,
            ParentId = parentId,
            Reload = reload
        });

        return Ok(result);
    }
}
