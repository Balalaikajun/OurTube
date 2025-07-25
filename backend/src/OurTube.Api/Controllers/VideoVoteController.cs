using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

/// <summary>
/// Работа с реакциями под видео.
/// </summary>
[Route("Video/{videoId:int}/vote")]
[ApiController]
public class VideoVoteController : ControllerBase
{
    private readonly IVideoVoteService _videoVoteVoteService;

    public VideoVoteController(IVideoVoteService videoVoteService)
    {
        _videoVoteVoteService = videoVoteService;
    }

    /// <summary>
    /// Поставить реакцию под видео.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <param name="type">Тип реакции.</param>
    /// <response code="201">Реакция сохранена.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Контент не найден.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> PostVote(Guid videoId,
        [FromBody] bool type)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            await _videoVoteVoteService.SetAsync(videoId, userId, type);
            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Удалить реакцию.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <response code="204">Реакция удалена.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Контент не найден.</response>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteDislike(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            await _videoVoteVoteService.DeleteAsync(videoId, userId);
            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}