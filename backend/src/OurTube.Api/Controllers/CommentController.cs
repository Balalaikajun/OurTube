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
        public async Task<ActionResult> Post(
            [FromBody] CommentPostDto postDto)
        {
            try
            {
                await _commentService.Create(
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
        public async Task<ActionResult> Patch(
            [FromBody] CommentPatchDto postDto)
        {
            try
            {
                await _commentService.Update(
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
        public async Task<ActionResult> Delete(
            int commentId)
        {
            try
            {
                await _commentService.Delete(
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
        public ActionResult GetWithLimit(
             int videoId,
             [FromQuery] int limit = 10,
             [FromQuery] int after = 0,
             [FromQuery] int? parentId = null)
        {
            try
            {
                var result =  _commentService.GetChildsWithLimit(
                videoId,
                limit,
                after,
                parentId);
                var nextAfter = after + limit;


                return Ok(new { result, nextAfter });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
