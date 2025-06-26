using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("Video/[Controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ICommentCrudService _commentCrudService;
    private readonly ICommentRecommendationService _commentRecommendationService;

    public CommentController(ICommentCrudService commentCrudService,
        ICommentRecommendationService commentRecommendationService)
    {
        _commentCrudService = commentCrudService;
        _commentRecommendationService = commentRecommendationService;
    }


    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CommentGetDto>> Post(
        [FromBody] CommentPostDto postDto)
    {
        try
        {
            var result = await _commentCrudService.CreateAsync(
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
            await _commentCrudService.UpdateAsync(
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
            await _commentCrudService.DeleteAsync(
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
        var sessionId = Request.Cookies["SessionId"];

        try
        {
            var result = await _commentRecommendationService.GetCommentsWithLimitAsync(
                videoId,
                limit,
                after,
                sessionId,
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