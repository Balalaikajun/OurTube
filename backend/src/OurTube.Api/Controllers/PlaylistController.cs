using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("[controller]")]
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
        var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistCrudService.CreateAsync(
            postDto,
            userId);
        return CreatedAtAction(
            nameof(GetByElements),
            new { id = result.Id },
            result);
    }

    [Authorize]
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Patch(
        Guid id,
        [FromBody] PlaylistPatchDto postDto)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _playlistCrudService.UpdateAsync(
                postDto,
                id,
                userId);

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
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(
        Guid id)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _playlistCrudService.DeleteAsync(
                id,
                userId);
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
    [HttpPost("{id:guid}/{videoId:guid}")]
    public async Task<ActionResult> AddVideo(
        Guid id,
        Guid videoId)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _playlistCrudService.AddVideoAsync(
                id,
                videoId,
                userId);
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
    [HttpDelete("{id:guid}/{videoId:guid}")]
    public async Task<ActionResult> RemoveVideo(
        Guid id,
        Guid videoId)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            await _playlistCrudService.RemoveVideoAsync(
                id,
                videoId,
                userId);
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
    [HttpGet("{id:guid}/elements")]
    public async Task<ActionResult<PagedDto<PlaylistElementGetDto>>> GetByElements(
        Guid id,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _playlistQueryService.GetElements(
                id,
                userId,
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
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PlaylistMinGetDto>> GetById(
        Guid id)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _playlistQueryService.GetMinById(id, userId);
            
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
        var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistQueryService.GetUserPlaylistsAsync(
            userId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("video/{videoId:guid}")]
    public async Task<ActionResult<IEnumerable<PlaylistForVideoGetDto>>> GetForVideo(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var result = await _playlistQueryService.GetUserPlaylistsForVideoAsync(userId, videoId);
        return Ok(result);
    }
}