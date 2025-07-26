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
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        var result = await _commentCrudService.CreateAsync(
            userId,
            postDto);
        
        return Created(
            string.Empty,
            result);
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Comment), FromBody = nameof(postDto.Id))]
    [HttpPatch]
    public async Task<ActionResult> Patch(
        [FromBody] CommentPatchDto postDto)
    {
        await _commentCrudService.UpdateAsync(postDto);
        
        return Created();
    }

    [Authorize]
    [IsUserHasAccessToEntity(typeof(Comment), FromRoute = nameof(commentId))]
    [HttpDelete("{commentId:guid}")]
    public async Task<ActionResult> Delete(Guid commentId)
    {
        await _commentCrudService.DeleteAsync(commentId);
        
        return Created();
    }

    [HttpGet("{videoId:guid}")]
    public async Task<ActionResult<PagedCommentDto>> GetWithLimit(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] Guid? parentId = null,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _commentRecommendationService.GetCommentsWithLimitAsync(new GetCommentsRequest
        (
            videoId,
            limit,
            after,
            sessionId,
            userId,
            parentId,
            reload
        ));

        return Ok(result);
    }
}