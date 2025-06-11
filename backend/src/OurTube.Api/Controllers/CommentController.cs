using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

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
    public async Task<ActionResult<CommentGetDto>> Post(
        [FromBody] CommentPostDto postDto)
    {
        try
        {
            var result = await _commentService.CreateAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                postDto);
            return Created(
                string.Empty,
                result);
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
    public async Task<ActionResult> Delete(
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
    public async Task<ActionResult<PagedCommentDto>> GetWithLimit(
        int videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] int? parentId = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await _commentService.GetChildrenWithLimitAsync(
                videoId,
                limit,
                after,
                userId,
                parentId);


            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}