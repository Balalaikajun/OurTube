using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers
{
    /// <summary>
    /// Работа с реакциями (голосами) под видео.
    /// </summary>
    [Route("Video/{videoId:int}/vote")]
    [ApiController]
    public class VideoVoteController : ControllerBase
    {
        private readonly IVideoVoteService _videoVoteService;

        public VideoVoteController(IVideoVoteService videoVoteService)
        {
            _videoVoteService = videoVoteService;
        }

        /// <summary>
        /// Поставить реакцию (лайк или дизлайк) под видео.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <param name="type">Тип реакции: true — лайк, false — дизлайк.</param>
        /// <response code="201">Реакция успешно сохранена.</response>
        /// <response code="400">Неверный формат входных данных.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        /// <response code="404">Видео не найдено.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostVote(Guid videoId, [FromBody] bool type)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _videoVoteService.SetAsync(videoId, userId, type);
            return Created();
        }

        /// <summary>
        /// Удалить реакцию под видео.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <response code="204">Реакция успешно удалена.</response>
        /// <response code="400">Неверный формат входных данных.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        /// <response code="404">Видео не найдено.</response>
        [Authorize]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteVote(Guid videoId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            await _videoVoteService.DeleteAsync(videoId, userId);
            return NoContent();
        }
    }
}
