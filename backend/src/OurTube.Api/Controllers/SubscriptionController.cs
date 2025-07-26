using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

/// <summary>
/// Работа с подписками пользователей.
/// </summary>
[Route("[controller]")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }


    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Subscribe(
        Guid userToId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _subscriptionService.SubscribeAsync(
            userId,
            userToId);

        return Created();
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> UnSubscribe(
        Guid userToId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _subscriptionService.UnSubscribeAsync(
            userToId,
            userToId);

        return Created();
    }
}