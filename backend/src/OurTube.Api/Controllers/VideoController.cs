using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

namespace OurTube.Api.Controllers
{
    /// <summary>
    /// Работа с видео
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        /// <summary>
        /// Конструктор контроллера VideoController.
        /// </summary>
        /// <param name="videoService">Сервис для работы с видео.</param>
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        /// <summary>
        /// Загрузить новое видео.
        /// </summary>
        /// <param name="request">Модель с данными для загрузки видео (multipart/form-data).</param>
        /// <param name="configuration">Сервис конфигурации приложения.</param>
        /// <returns>Минимальные данные загруженного видео.</returns>
        /// <response code="201">Видео успешно создано и возвращены его минимальные данные.</response>
        /// <response code="400">Неверный формат входных данных.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(MinVideo), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<MinVideo>> Post(
            [FromForm] PostVideoRequest request,
            [FromServices] IConfiguration configuration)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _videoService.PostVideo(
                request,
                userId);

            return CreatedAtAction(
                nameof(Get),
                new { videoId = result.Id },
                result);
        }

        /// <summary>
        /// Получает подробную информацию о видео по его идентификатору.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <returns>Полные данные видео.</returns>
        /// <response code="200">Видео найдено и возвращены его данные.</response>
        /// <response code="404">Видео с указанным идентификатором не найдено.</response>
        [HttpGet("{videoId:guid}")]
        [ProducesResponseType(typeof(Video), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Video>> Get(Guid videoId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var video = await _videoService.GetVideoByIdAsync(videoId, userId);
            return Ok(video);
        }
    }
}