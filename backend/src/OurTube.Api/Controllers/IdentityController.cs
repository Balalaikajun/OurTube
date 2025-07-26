using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Replies.Common;

namespace OurTube.Api.Controllers;

/// <summary>
/// Управление аутентификацией пользователя.
/// </summary>
[Route("identity")]
[ApiController]
public class IdentityController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    /// <summary>
    /// Выйти из системы (разлогиниться).
    /// </summary>
    /// <response code="204">Пользователь успешно разлогинен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="500">Неизвестная ошибка сервера.</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}