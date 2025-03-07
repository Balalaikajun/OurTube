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
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PlaylistPostDTO postDTO, [FromServices] PlaylistService playlistService)
        {

            await playlistService.Create(
                postDTO,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Created();
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] PlaylistPatchDTO postDTO, [FromServices] PlaylistService playlistService)
        {
            try
            {


                await playlistService.Update(
                    postDTO,
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
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id, [FromServices] PlaylistService playlistService)
        {

            try
            {
                await playlistService.Delete(
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
        [HttpPost("{id}/{videoId}")]
        public async Task<ActionResult> AddVideo(int id, int videoId, [FromServices] PlaylistService playlistService)
        {
            try
            {
                await playlistService.AddVideo(
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
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}/{videoId}")]
        public async Task<ActionResult> RemoveVideo(int id, int videoId, [FromServices] PlaylistService playlistService)
        {
            try
            {
            await playlistService.RemoveVideo(
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
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(
            int id,
            [FromServices] PlaylistService playlistService, 
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            try
            {
                PlaylistGetDTO playlistGetDTO = playlistService.GetWithLimit(
                id,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                limit,
                after);
                int nextAfter = after + limit;


                return Ok(new { playlistGetDTO, nextAfter });
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
        public async Task<ActionResult> Get([FromServices] PlaylistService playlistService)
        {
            List<PlaylistMinGetDTO> playlists = playlistService.GetUserPlaylists(
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(playlists);
        }


    }
}
