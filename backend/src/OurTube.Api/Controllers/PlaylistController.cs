using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.DTOs.Common;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpPatch("{playlistId:guid}")]
    public async Task<ActionResult> Patch(
        Guid playlistId,
        [FromBody] PlaylistPatchDto postDto)
    {
        try
        {
            await _playlistCrudService.UpdateAsync(
                postDto,
                playlistId);

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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}")]
    public async Task<ActionResult> Delete(Guid playlistId)
    {
        try
        {
            await _playlistCrudService.DeleteAsync(playlistId);
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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpPost("{playlistId:guid}/{videoId:guid}")]
    public async Task<ActionResult> AddVideo(
        Guid playlistId,
        Guid videoId)
    {
        try
        {
            await _playlistCrudService.AddVideoAsync(
                playlistId,
                videoId);
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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}/{videoId:guid}")]
    public async Task<ActionResult> RemoveVideo(
        Guid playlistId,
        Guid videoId)
    {
        try
        {
            await _playlistCrudService.RemoveVideoAsync(
                playlistId,
                videoId);
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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:guid}/elements")]
    public async Task<ActionResult<PagedDto<PlaylistElementGetDto>>> GetByElements(
        Guid playlistId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0)
    {
        try
        {
            var userId= Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _playlistQueryService.GetElements(
                playlistId,
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
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:int}")]
    public async Task<ActionResult<PlaylistMinGetDto>> GetById(Guid playlistId)
    {
        try
        {
            var result = await _playlistQueryService.GetMinById(playlistId);
            
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