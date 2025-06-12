using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VideoController : ControllerBase
{
    private readonly VideoService _videoService;

    public VideoController(VideoService videoService)
    {
        _videoService = videoService;
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<VideoMinGetDto>> Post(
        [FromForm] VideoUploadDto videoUploadDto,
        [FromServices] IConfiguration configuration)
    {
        try
        {
            var result = await _videoService.PostVideo(
                videoUploadDto,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return CreatedAtAction(
                nameof(Get),
                new { videoId = result.Id },
                result);
        }
        catch (FormatException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{videoId:int}")]
    public async Task<ActionResult<VideoGetDto>> Get(int videoId, VideoService videoService)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            return Ok(userId != null
                ? await videoService.GetVideoByIdAsync(videoId, userId)
                : await videoService.GetVideoByIdAsync(videoId));
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}