using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;

namespace OurTube.Api.Controllers;

/// <summary>
///     Работа с подписками пользователей.
/// </summary>
[Route("users/{userToId:guid}")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    /// <summary>
    ///     Подписаться на пользователя.
    /// </summary>
    /// <param name="userToId">Идентификатор пользователя, на которого подписываемся.</param>
    /// <response code="201">Подписка успешно создана.</response>
    /// <response code="400">Неверные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [HttpPost("subscribe")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Subscribe(Guid userToId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _subscriptionService.SubscribeAsync(userId, userToId);

        return Created();
    }

    /// <summary>
    ///     Отписаться от пользователя.
    /// </summary>
    /// <param name="userToId">Идентификатор пользователя, от которого отписываемся.</param>
    /// <response code="201">Отписка успешно выполнена.</response>
    /// <response code="400">Неверные данные запроса.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    /// <response code="404">Элемент не найден.</response>
    /// <response code="500">Ошибка сервера.</response>
    [Authorize]
    [HttpDelete("unsubscribe")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnSubscribe(Guid userToId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _subscriptionService.UnSubscribeAsync(userId, userToId);

        return Created();
    }
}