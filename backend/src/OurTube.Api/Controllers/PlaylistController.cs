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
        public async Task<ActionResult> Post(
            [FromBody] PlaylistPostDto postDto)
        {

            await _playlistService.Create(
                postDto,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Created();
        }

        [Authorize]
        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(
            int id,
            [FromBody] PlaylistPatchDto postDto)
        {
            try
            {


                await _playlistService.Update(
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
                await _playlistService.Delete(
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
                await _playlistService.AddVideo(
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
                await _playlistService.RemoveVideo(
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
        public async Task<ActionResult> Get(
            int id,
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            try
            {
                var playlistGetDto = await _playlistService.GetWithLimit(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                limit,
                after);
                var nextAfter = after + limit;


                return Ok(new { playlistGetDto, nextAfter });
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
        public ActionResult<IEnumerable<PlaylistMinGetDto>> Get()
        {
            var result = _playlistService.GetUserPlaylists(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(result);
        }


    }
}
