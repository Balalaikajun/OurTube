using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Playlist;
using OurTube.Application.Requests.Playlist;
using Playlist = OurTube.Domain.Entities.Playlist;
using PlaylistElement = OurTube.Application.Replies.PlaylistElement.PlaylistElement;

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
    public async Task<ActionResult<Application.Replies.Playlist.Playlist>> Post(
        [FromBody] PostPlaylistRequest postDto)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

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
        [FromBody] UpdatePlaylistRequest postDto)
    {
        await _playlistCrudService.UpdateAsync(
            postDto,
            playlistId);

        return Ok();
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}")]
    public async Task<ActionResult> Delete(Guid playlistId)
    {
        await _playlistCrudService.DeleteAsync(playlistId);
        
        return Ok();
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpPost("{playlistId:guid}/{videoId:guid}")]
    public async Task<ActionResult> AddVideo(
        Guid playlistId,
        Guid videoId)
    {
        await _playlistCrudService.AddVideoAsync(
            playlistId,
            videoId);

        return Ok();
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpDelete("{playlistId:guid}/{videoId:guid}")]
    public async Task<ActionResult> RemoveVideo(
        Guid playlistId,
        Guid videoId)
    {
        await _playlistCrudService.RemoveVideoAsync(
            playlistId,
            videoId);

        return Ok();
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:guid}/elements")]
    public async Task<ActionResult<ListReply<PlaylistElement>>> GetByElements(
        Guid playlistId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistQueryService.GetElements(
            playlistId,
            userId,
            limit,
            after);

        return Ok(result);
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Playlist), FromRoute = nameof(playlistId))]
    [HttpGet("{playlistId:int}")]
    public async Task<ActionResult<Application.Replies.Playlist.Playlist>> GetById(Guid playlistId)
    {
        var result = await _playlistQueryService.GetMinById(playlistId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Application.Replies.Playlist.Playlist>>> GetUserPlaylists()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _playlistQueryService.GetUserPlaylistsAsync(
            userId);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("video/{videoId:guid}")]
    public async Task<ActionResult<IEnumerable<PlaylistForVideo>>> GetForVideo(Guid videoId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        var result = await _playlistQueryService.GetUserPlaylistsForVideoAsync(userId, videoId);
        
        return Ok(result);
    }
}