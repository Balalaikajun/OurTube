using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<PlaylistMinGetDto>> Post(
        [FromBody] PlaylistPostDto postDto)
    {
        var result = await _playlistCrudService.CreateAsync(
            postDto,
            User.FindFirstValue(ClaimTypes.NameIdentifier));
        return CreatedAtAction(
            nameof(GetByElements),
            new { id = result.Id },
            result);
    }

    [Authorize]
    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(
        int id,
        [FromBody] PlaylistPatchDto postDto)
    {
        try
        {
            await _playlistCrudService.UpdateAsync(
                postDto,
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(
        int id)
    {
        try
        {
            await _playlistCrudService.DeleteAsync(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{id:int}/{videoId:int}")]
    public async Task<ActionResult> AddVideo(
        int id,
        int videoId)
    {
        try
        {
            await _playlistCrudService.AddVideoAsync(
                id,
                videoId,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:int}/{videoId:int}")]
    public async Task<ActionResult> RemoveVideo(
        int id,
        int videoId)
    {
        try
        {
            await _playlistCrudService.RemoveVideoAsync(
                id,
                videoId,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Ok();
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PagedDto<PlaylistElementGetDto>>> GetByElements(
        int id,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0)
    {
        try
        {
            var result = await _playlistQueryService.GetElements(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                limit,
                after);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlaylistMinGetDto>>> GetUserPlaylists()
    {
        var result = await _playlistQueryService.GetUserPlaylistsAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier));

        return Ok(result);
    }

    [Authorize]
    [HttpGet("video/{videoId:int}")]
    public async Task<ActionResult<IEnumerable<PlaylistForVideoGetDto>>> GetForVideo(int videoId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _playlistQueryService.GetUserPlaylistsForVideoAsync(userId, videoId);
        return Ok(result);
    }
}