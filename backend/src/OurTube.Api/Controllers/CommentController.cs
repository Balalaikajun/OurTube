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
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }
        
        
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostAsync(
            [FromBody] CommentPostDto postDto)
        {
            try
            {
                await _commentService.CreateAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDto);
                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> PatchAsync(
            [FromBody] CommentPatchDto postDto)
        {
            try
            {
                await _commentService.UpdateAsync(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDto);
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
        [HttpDelete("{commentId:int}")]
        public async Task<ActionResult> DeleteAsync(
            int commentId)
        {
            try
            {
                await _commentService.DeleteAsync(
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

        [HttpGet("{videoId:int}")]
        public async Task<ActionResult<PagedCommentDto>> GetWithLimitAsync(
             int videoId,
             [FromQuery] int limit = 10,
             [FromQuery] int after = 0,
             [FromQuery] int? parentId = null)
        {
            try
            {
                var result = await _commentService.GetChildsWithLimitAsync(
                videoId,
                limit,
                after,
                parentId);
                var nextAfter = after + limit;


                return Ok(new PagedCommentDto()
                {
                    Comments = result,
                    NextAfter = nextAfter
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
