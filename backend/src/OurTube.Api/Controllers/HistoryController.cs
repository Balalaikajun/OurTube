using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Views;

namespace OurTube.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly IViewService _viewService;

    public HistoryController(IViewService viewService)
    {
        _viewService = viewService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult> AddVideo(
        [FromBody] PostViewsRequest postDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.AddVideoAsync(
            postDto,
            userId);

        return Created();
    }

    [Authorize]
    [HttpDelete("{videoId:guid}")]
    public async Task<ActionResult> RemoveVideo(
        Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.RemoveVideoAsync(
            videoId,
            userId);

        return Ok();
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> ClearHistory()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _viewService.ClearHistoryAsync(
            userId);

        return Ok();
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] string? query = null)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _viewService.GetWithLimitAsync(
            userId,
            limit,
            after,
            query);

        return Ok(result);
    }
}