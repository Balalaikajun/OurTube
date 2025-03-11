using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/Video/[Controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post(
            [FromBody] CommentPostDTO postDTO,
            [FromServices] CommentService commentService)
        {
            try
            {
                await commentService.Create(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDTO);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> Patch(
            [FromBody] CommentPatchDTO postDTO,
            [FromServices] CommentService commentService)
        {
            try
            {
                await commentService.Update(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDTO);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<ActionResult> Delete(
            int commentId,
            [FromServices] CommentService commentService)
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
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{videoId}")]
        public async Task<ActionResult> GetWithLimit(
             int videoId,
             [FromServices] CommentService commentService,
             [FromQuery] int limit = 10,
             [FromQuery] int after = 0,
             [FromQuery] int? parentId = null)
        {
            try
            {
                List<CommentGetDTO> result = await commentService.GetChildsWithLimit(
                videoId,
                limit,
                after,
                parentId);
                int nextAfter = after + limit;


                return Ok(new { result, nextAfter });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
