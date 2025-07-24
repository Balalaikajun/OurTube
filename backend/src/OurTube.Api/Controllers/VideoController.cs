using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;

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
        /// <param name="videoUploadDto">Модель с данными для загрузки видео (multipart/form-data).</param>
        /// <param name="configuration">Сервис конфигурации приложения.</param>
        /// <returns>Минимальные данные загруженного видео.</returns>
        /// <response code="201">Видео успешно создано и возвращены его минимальные данные.</response>
        /// <response code="400">Неверный формат входных данных.</response>
        /// <response code="401">Пользователь не авторизован.</response>
        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(VideoMinGetDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<VideoMinGetDto>> Post(
            [FromForm] VideoUploadDto videoUploadDto,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _videoService.PostVideo(
                    videoUploadDto,
                    userId);

                return CreatedAtAction(
                    nameof(Get),
                    new { videoId = result.Id },
                    result);
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Получает подробную информацию о видео по его идентификатору.
        /// </summary>
        /// <param name="videoId">Идентификатор видео.</param>
        /// <returns>Полные данные видео.</returns>
        /// <response code="200">Видео найдено и возвращены его данные.</response>
        /// <response code="404">Видео с указанным идентификатором не найдено.</response>
        [HttpGet("{videoId:int}")]
        [ProducesResponseType(typeof(VideoGetDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VideoGetDto>> Get(int videoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                var video = await _videoService.GetVideoByIdAsync(videoId, userId);
                return Ok(video);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
