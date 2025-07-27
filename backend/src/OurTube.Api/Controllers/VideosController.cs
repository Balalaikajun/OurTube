using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

namespace OurTube.Api.Controllers;

/// <summary>
///     Работа с видео.
/// </summary>
[Route("[controller]")]
[ApiController]
[Authorize] // Если все методы требуют авторизации
public class VideosController : ControllerBase
{
    private readonly IVideoService _videoService;

    /// <summary>
    ///     Конструктор контроллера VideoController.
    /// </summary>
    /// <param name="videoService">Сервис для работы с видео.</param>
    public VideosController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    /// <summary>
    ///     Загрузить новое видео.
    /// </summary>
    /// <param name="request">Модель с данными для загрузки видео (multipart/form-data).</param>
    /// <returns>Минимальные данные загруженного видео.</returns>
    /// <response code="201">Видео успешно создано и возвращены его минимальные данные.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MinVideo), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<MinVideo>> Post([FromForm] PostVideoRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _videoService.PostVideo(request, userId);

        return CreatedAtAction(
            nameof(Get),
            new { videoId = result.Id },
            result);
    }

    /// <summary>
    ///     Получить подробную информацию о видео по его идентификатору.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <returns>Полные данные видео.</returns>
    /// <response code="200">Видео найдено и возвращены его данные.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Видео с указанным идентификатором не найдено.</response>
    /// <response code="500">Ошибка сервера.</response>
    [HttpGet("{videoId:guid}")]
    [ProducesResponseType(typeof(Video), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Video>> Get(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var video = await _videoService.GetVideoByIdAsync(videoId, userId);
        return Ok(video);
    }
}