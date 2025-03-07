using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using OurTube.Infrastructure.Data;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video")]
    public class VideoVoteController : ControllerBase
    {

        [Authorize]
        [HttpPost("{videoId}/like")]
        public async Task<ActionResult> PostLike(int videoId, [FromServices]VideoVoteService voteService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await voteService.Set(videoId, userId, true);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost("{videoId}/deslike")]
        public async Task<ActionResult> PostDeslike(int videoId, [FromServices] VideoVoteService voteService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await voteService.Set(videoId, userId, false);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpDelete("{videoId}/like")]
        public async Task<ActionResult> DeleteLike(int videoId, [FromServices] VideoVoteService voteService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await voteService.Delete(videoId, userId);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [Authorize]
        [HttpDelete("{videoId}/deslike")]
        public async Task<ActionResult> DeleteDeslike(int videoId, [FromServices] VideoVoteService voteService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await voteService.Delete(videoId, userId);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
