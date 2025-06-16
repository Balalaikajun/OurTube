using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
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
        string userToId)
    {
        try
        {
            await _subscriptionService.SubscribeAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                userToId);

            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> UnSubscribe(
        string userToId)
    {
        try
        {
            await _subscriptionService.UnSubscribeAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                userToId);

            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}