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
    [ApiController]
    public class VoteController : ControllerBase
    {

        [Authorize]
        [HttpPost("{videoId}/like")]
        public async Task<ActionResult> PostLike(int videoId, VoteService voteService)
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
        public async Task<ActionResult> PostDeslike(int videoId, VoteService voteService)
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
        public async Task<ActionResult> DeleteLike(int videoId, VoteService voteService)
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
        public async Task<ActionResult> DeleteDeslike(int videoId, VoteService voteService)
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
