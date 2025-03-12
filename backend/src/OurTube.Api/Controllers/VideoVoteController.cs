using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video")]
    public class VideoVoteController : ControllerBase
    {
        private readonly VideoVoteService _videoVoteVoteService;

        public VideoVoteController(VideoVoteService videoVoteService)
        {
            _videoVoteVoteService = videoVoteService;
        }

        [Authorize]
        [HttpPost("{videoId:int}/vote")]
        public async Task<ActionResult> PostVote(int videoId,
            [FromBody] bool type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _videoVoteVoteService.Set(videoId, userId, type);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpDelete("{videoId:int}/vote")]
        public async Task<ActionResult> DeleteDislike(int videoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _videoVoteVoteService.Delete(videoId, userId);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
