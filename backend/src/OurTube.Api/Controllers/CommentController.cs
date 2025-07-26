using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var result = await _commentCrudService.CreateAsync(
                userId,
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
    [IsUserHasAccessToEntity(typeof(Comment), FromBody = nameof(postDto.Id))]
    [HttpPatch]
    public async Task<ActionResult> Patch(
        [FromBody] CommentPatchDto postDto)
    {
        try
        {
            await _commentCrudService.UpdateAsync(postDto);
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
    [IsUserHasAccessToEntity(typeof(Comment), FromRoute = nameof(commentId))]
    [HttpDelete("{commentId:guid}")]
    public async Task<ActionResult> Delete(Guid commentId)
    {
        try
        {
            await _commentCrudService.DeleteAsync(commentId);
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

    [HttpGet("{videoId:guid}")]
    public async Task<ActionResult<PagedCommentDto>> GetWithLimit(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] Guid? parentId = null)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

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