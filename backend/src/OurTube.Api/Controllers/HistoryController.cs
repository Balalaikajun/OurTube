using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Views;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HistoryController : ControllerBase
{
    private readonly ViewService _viewService;

    public HistoryController(ViewService viewService)
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
            await _viewService.AddVideoAsync(
                postDto.VideoId,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                postDto.EndTime);

            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{videoId:int}")]
    public async Task<ActionResult> RemoveVideo(
        int videoId)
    {
        try
        {
            await _viewService.RemoveVideoAsync(
                videoId,
                User.FindFirstValue(ClaimTypes.NameIdentifier));

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
            await _viewService.ClearHistoryAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedHistoryDto>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0)
    {
        try
        {
            var history = await _viewService.GetWithLimitAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                limit,
                after);
            var nextAfter = after + limit;


            return Ok(new PagedHistoryDto
            {
                Views = history,
                NextAfter = nextAfter
            });
        }

        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}