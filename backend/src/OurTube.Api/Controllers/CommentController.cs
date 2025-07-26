using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Api.Attributes;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Requests.Comment;
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
    public async Task<ActionResult<Application.Replies.Comment.Comment>> Post(
        [FromBody] PostCommentRequest postDto)
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
        [FromBody] UpdateCommentRequest postDto)
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
    public async Task<ActionResult<ListReply<Application.Replies.Comment.Comment>>> GetWithLimit(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] Guid? parentId = null,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);

        var result = await _commentRecommendationService.GetCommentsWithLimitAsync(new GetCommentRequest
        {
            VideoId = videoId,
            Limit = limit,
            After = after,
            SessionId = sessionId,
            UserId = userId,
            ParentId = parentId,
            Reload = reload
        });

        return Ok(result);
    }
}