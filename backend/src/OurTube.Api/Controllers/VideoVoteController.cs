using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video/{videoId:int}/vote")]
    public class VideoVoteController : ControllerBase
    {
        private readonly VideoVoteService _videoVoteVoteService;

        public VideoVoteController(VideoVoteService videoVoteService)
        {
            _videoVoteVoteService = videoVoteService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostVoteAsync(int videoId,
            [FromBody] bool type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _videoVoteVoteService.SetAsync(videoId, userId, type);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> DeleteDislikeAsync(int videoId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                await _videoVoteVoteService.DeleteAsync(videoId, userId);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
