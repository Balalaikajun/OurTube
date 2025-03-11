using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video")]
    public class VideoVoteController : ControllerBase
    {

        [Authorize]
        [HttpPost("{videoId}/vote")]
        public async Task<ActionResult> PostVote(int videoId,
            [FromBody] bool type,
            [FromServices] VideoVoteService voteService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await voteService.Set(videoId, userId, type);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpDelete("{videoId}/vote")]
        public async Task<ActionResult> DeleteDislike(int videoId, [FromServices] VideoVoteService voteService)
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
