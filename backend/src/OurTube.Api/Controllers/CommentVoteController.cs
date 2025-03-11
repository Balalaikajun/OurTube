using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video/Comment")]
    [ApiController]
    public class CommentVoteController : ControllerBase
    {

        [Authorize]
        [HttpPost("{commentId}/vote")]
        public async Task<ActionResult> PostVote(
            int commentId, 
            [FromBody] bool type, 
        [FromServices] CommentVoteService commentService)
        {
            try
            {
                await commentService.Set(
                    commentId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    type);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [Authorize]
        [HttpDelete("{commentId}/vote")]
        public async Task<ActionResult> DeleteVote(int commentId, CommentVoteService commentService)
        {
            try
            {
                await commentService.Delete(
                    commentId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
