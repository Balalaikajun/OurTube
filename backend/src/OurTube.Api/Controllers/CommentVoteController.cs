using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;

namespace OurTube.Api.Controllers;

/// <summary>
/// Управление голосами за комментарии (лайки и дизлайки).
/// </summary>
[Route("Video/Comment/{commentId:guid}/vote")]
[ApiController]
public class CommentVoteController : ControllerBase
{
    private readonly ICommentVoteService _commentVoteService;

    public CommentVoteController(ICommentVoteService commentVoteService)
    {
        _commentVoteService = commentVoteService;
    }

    /// <summary>
    /// Проголосовать за комментарий (лайк или дизлайк).
    /// </summary>
    /// <param name="commentId">Идентификатор комментария.</param>
    /// <param name="type">Тип голоса: <c>true</c> – лайк, <c>false</c> – дизлайк.</param>
    /// <response code="204">Голос успешно установлен или обновлён.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Комментарий не найден.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PostVote(
        Guid commentId,
        [FromBody] bool type)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _commentVoteService.SetAsync(
            commentId,
            userId,
            type);

        return NoContent();
    }

    /// <summary>
    /// Удалить голос за комментарий.
    /// </summary>
    /// <param name="commentId">Идентификатор комментария.</param>
    /// <response code="204">Голос успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Комментарий не найден или голос отсутствует.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpDelete("")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteVote(Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _commentVoteService.DeleteAsync(
            commentId,
            userId);

        return NoContent();
    }
}
