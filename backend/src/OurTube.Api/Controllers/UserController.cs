using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.ApplicationUser;
using OurTube.Application.Replies.UserAvatar;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.ApplicationUser;
using OurTube.Application.Requests.UserAvatar;

namespace OurTube.Api.Controllers;

/// <summary>
/// Работа с пользователем
/// </summary>
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserAvatarService _userAvatarService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IUserAvatarService userAvatarService)
    {
        _userService = userService;
        _userAvatarService = userAvatarService;
    }

    /// <summary>
    /// Обновить данные пользователя.
    /// </summary>
    /// <param name="request">Запрос на обновление данных пользователя.</param>
    /// <returns>Минимальные данные обновлённого аккаунта.</returns>
    /// <response code="201">Данные пользователя успешно обновлены и возвращены его минимальные данные.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Пользователь с такими данными не найден.</response>
    [Authorize]
    [HttpPatch]
    [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Application.Replies.ApplicationUser.ApplicationUser>> Patch(
        [FromBody] PatchApplicationUserRequest request)
    {
        var userId = Guid.Parse((ReadOnlySpan<char>)User.FindFirstValue(ClaimTypes.NameIdentifier));

        var result = await _userService.UpdateUserAsync(request, userId);
        return Created(
            string.Empty,
            result);
    }

    /// <summary>
    /// Получить данные пользователя.
    /// </summary>
    /// <returns>Данные пользователя.</returns>
    /// <remarks>Для корректной работы требуется наличие идентификатора пользователя в запросе.</remarks>
    /// <response code="200">Пользователь найден, его данные возвращены.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(ApplicationUser), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApplicationUser>> Get()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        return Ok(await _userService.GetUserAsync(userId));
    }

    /// <summary>
    /// Обновить аватар пользователя.
    /// </summary>
    /// <param name="request">Запрос на обновление аватара пользователя.</param>
    /// <returns>Данные обновлённого аватара пользователя.</returns>
    /// <response code="200">Пользователь найден, его данные возвращены.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [Authorize]
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(MinVideo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserAvatar>> CreateOrUpdateAvatar([FromForm] PostUserAvatarRequest request)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _userAvatarService.CreateOrUpdateUserAvatarAsync(request.Image, userId);

        return Created(string.Empty, result);
    }

    /// <summary>
    /// Удалить аватар пользователя.
    /// </summary>
    /// <response code="204">Аватар пользователя успешно удалён.</response>
    /// <response code="400">Неверный формат входных данных.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Контент не найден.</response>
    [Authorize]
    [HttpDelete("avatar")]
    [ProducesResponseType(typeof(MinVideo), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAvatar()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _userAvatarService.DeleteUserAvatarAsync(userId);

        return NoContent();
    }
}