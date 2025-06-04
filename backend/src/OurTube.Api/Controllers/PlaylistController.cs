using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService _playlistService;

        public PlaylistController(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostAsync(
            [FromBody] PlaylistPostDto postDto)
        {
            await _playlistService.CreateAsync(
                postDto,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Created();
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> PatchAsync(
            int id,
            [FromBody] PlaylistPatchDto postDto)
        {
            try
            {
                await _playlistService.UpdateAsync(
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
        public async Task<ActionResult> DeleteAsync(
            int id)
        {
            try
            {
                await _playlistService.DeleteAsync(
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
        public async Task<ActionResult> AddVideoAsync(
            int id,
            int videoId)
        {
            try
            {
                await _playlistService.AddVideoAsync(
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
        public async Task<ActionResult> RemoveVideoAsync(
            int id,
            int videoId)
        {
            try
            {
                await _playlistService.RemoveVideoAsync(
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
        public async Task<ActionResult<PagedPlaylistDto>> GetAsync(
            int id,
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            try
            {
                var playlistGetDto = await _playlistService.GetWithLimitAsync(
                    id,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    limit,
                    after);
                var nextAfter = after + limit;


                return Ok(new PagedPlaylistDto()
                {
                    Playlist = playlistGetDto,
                    NextAfter = nextAfter
                });
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
        public async Task<ActionResult<IEnumerable<PlaylistMinGetDto>>> GetAsync()
        {
            var result = await _playlistService.GetUserPlaylistsAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(result);
        }

        [Authorize]
        [HttpGet("video/{videoId:int}")]
        public async Task<ActionResult<IEnumerable<PlaylistForVideoGetDto>>> GetForVideoAsync(int videoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _playlistService.GetUserPlaylistsForVideoAsync(userId, videoId);
            return Ok(result);
        }
    }
}