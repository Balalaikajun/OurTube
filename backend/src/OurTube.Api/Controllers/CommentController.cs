using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.DTOs.Playlist;
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
            [FromServices]CommentService commentService)
        {
            try
            {
                await commentService.Create(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDTO);
                return Created();
            }
            catch(InvalidOperationException ex)
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
        [HttpDelete]
        public async Task<ActionResult> Delete(
            [FromBody] int id,
            [FromServices] CommentService commentService)
        {
            try
            {
                await commentService.Delete(
                    id,
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


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(
            int id,
            [FromServices] CommentService commentService,
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            try
            {
                List<CommentGetDTO> result = await commentService.GetWithLimit(
                id,
                limit,
                after);
                int nextAfter = after + limit;


                return Ok(new { result, nextAfter });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/childs")]
        public async Task<ActionResult> GetChilds(
             int id,
             [FromServices] CommentService commentService,
             [FromQuery] int limit = 10,
             [FromQuery] int after = 0)
        {
            try
            {
                List<CommentGetDTO> result = await commentService.GetChildsWithLimit(
                id,
                limit,
                after);
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
