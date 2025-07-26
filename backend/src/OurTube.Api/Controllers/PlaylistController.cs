using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Playlist;
using OurTube.Application.Requests.Playlist;
using Playlist = OurTube.Domain.Entities.Playlist;
using PlaylistElement = OurTube.Application.Replies.PlaylistElement.PlaylistElement;

namespace OurTube.Api.Controllers;

/// <summary>
/// Управление плейлистами пользователя.
/// </summary>
[Route("[controller]")]
[ApiController]
public class PlaylistController : ControllerBase
{
    private readonly IPlaylistCrudService _playlistCrudService;
    private readonly IPlaylistQueryService _playlistQueryService;

    public PlaylistController(IPlaylistCrudService playlistCrudService, IPlaylistQueryService playlistQueryService)
    {
        _playlistCrudService = playlistCrudService;
        _playlistQueryService = playlistQueryService;
    }

    /// <summary>
    /// Создать новый плейлист.
    /// </summary>
    /// <param name="postDto">Данные для создания плейлиста.</param>
    /// <returns>Созданный плейлист с минимальными данными.</returns>
    /// <response code="201">Плейлист успешно создан.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(Application.Replies.Playlist.Playlist), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Application.Replies.Playlist.Playlist>> Post(
        [FromBody] PostPlaylistRequest postDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistCrudService.CreateAsync(postDto, userId);

        return CreatedAtAction(nameof(GetByElements), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновить плейлист.
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <param name="postDto">Данные для обновления плейлиста.</param>
    /// <response code="200">Плейлист успешно обновлён.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpPatch("{playlistId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Patch(Guid playlistId, [FromBody] UpdatePlaylistRequest postDto)
    {
        await _playlistCrudService.UpdateAsync(postDto, playlistId);
        return Ok();
    }

    /// <summary>
    /// Удалить плейлист.
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <response code="200">Плейлист успешно удалён.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete(Guid playlistId)
    {
        await _playlistCrudService.DeleteAsync(playlistId);
        return Ok();
    }

    /// <summary>
    /// Добавить видео в плейлист.
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <response code="200">Видео успешно добавлено в плейлист.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист или видео не найдены.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpPost("{playlistId:guid}/{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> AddVideo(Guid playlistId, Guid videoId)
    {
        await _playlistCrudService.AddVideoAsync(playlistId, videoId);
        return Ok();
    }

    /// <summary>
    /// Удалить видео из плейлиста.
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <response code="200">Видео успешно удалено из плейлиста.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист или видео не найдены.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}/{videoId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveVideo(Guid playlistId, Guid videoId)
    {
        await _playlistCrudService.RemoveVideoAsync(playlistId, videoId);
        return Ok();
    }

    /// <summary>
    /// Получить элементы плейлиста с пагинацией.
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <param name="limit">Количество элементов для загрузки (по умолчанию 10).</param>
    /// <param name="after">Смещение по количеству элементов (для пагинации).</param>
    /// <returns>Список элементов плейлиста.</returns>
    /// <response code="200">Элементы плейлиста успешно получены.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:guid}/elements")]
    [ProducesResponseType(typeof(ListReply<PlaylistElement>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ListReply<PlaylistElement>>> GetByElements(Guid playlistId, [FromQuery] int limit = 10, [FromQuery] int after = 0)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistQueryService.GetElements(playlistId, userId, limit, after);

        return Ok(result);
    }

    /// <summary>
    /// Получить плейлист по идентификатору (минимальные данные).
    /// </summary>
    /// <param name="playlistId">Идентификатор плейлиста.</param>
    /// <returns>Минимальные данные плейлиста.</returns>
    /// <response code="200">Плейлист успешно получен.</response>
    /// <response code="401">Пользователь не авторизован или не имеет доступа.</response>
    /// <response code="404">Плейлист не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:guid}")]
    [ProducesResponseType(typeof(Application.Replies.Playlist.Playlist), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Application.Replies.Playlist.Playlist>> GetById(Guid playlistId)
    {
        var result = await _playlistQueryService.GetMinById(playlistId);
        return Ok(result);
    }

    /// <summary>
    /// Получить все плейлисты текущего пользователя.
    /// </summary>
    /// <returns>Список плейлистов пользователя.</returns>
    /// <response code="200">Плейлисты успешно получены.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Application.Replies.Playlist.Playlist>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Application.Replies.Playlist.Playlist>>> GetUserPlaylists()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistQueryService.GetUserPlaylistsAsync(userId);

        return Ok(result);
    }

    /// <summary>
    /// Получить плейлисты пользователя, содержащие указанное видео.
    /// </summary>
    /// <param name="videoId">Идентификатор видео.</param>
    /// <returns>Список плейлистов, содержащих видео.</returns>
    /// <response code="200">Плейлисты успешно получены.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [HttpGet("video/{videoId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<PlaylistForVideo>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<PlaylistForVideo>>> GetForVideo(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var result = await _playlistQueryService.GetUserPlaylistsForVideoAsync(userId, videoId);

        return Ok(result);
    }
}
