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
        [HttpPost("{commentId}/like")]
        public async Task<ActionResult> PostLike(int commentId, CommentVoteService commentService)
        {
            try
            {
                await commentService.Set(
                    commentId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier), 
                    true);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost("{commentId}/deslike")]
        public async Task<ActionResult> PostDeslike(int commentId, CommentVoteService commentService)
        {
            try
            {
                await commentService.Set(
                    commentId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier), 
                    false);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpDelete("{commentId}/like")]
        public async Task<ActionResult> DeleteLike(int commentId, CommentVoteService commentService)
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


        [Authorize]
        [HttpDelete("{commentId}/deslike")]
        public async Task<ActionResult> DeleteDeslike(int commentId, CommentVoteService commentService)
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
