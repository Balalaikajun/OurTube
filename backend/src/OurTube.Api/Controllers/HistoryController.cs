using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.Views;
using OurTube.Application.Interfaces;

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
        [FromBody] ViewPostDto postDto)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _viewService.AddVideoAsync(
                postDto,
                userId);

            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{videoId:guid}")]
    public async Task<ActionResult> RemoveVideo(
        Guid videoId)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _viewService.RemoveVideoAsync(
                videoId,
                userId);

            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<ActionResult> ClearHistory()
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _viewService.ClearHistoryAsync(
                userId);

            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedVideoDto>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] string? query = null)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var result = await _viewService.GetWithLimitAsync(
                userId,
                limit,
                after,
                query);

            return Ok(result);
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}