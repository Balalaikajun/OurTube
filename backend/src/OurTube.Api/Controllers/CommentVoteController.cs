using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("Video/Comment/{commentId:int}/vote")]
[ApiController]
public class CommentVoteController : ControllerBase
{
    private readonly ICommentVoteService _commentVoteService;

    public CommentVoteController(ICommentVoteService commentVoteService)
    {
        _commentVoteService = commentVoteService;
    }

    [Authorize]
    [HttpPost("")]
    public async Task<ActionResult> PostVote(
        Guid commentId,
        [FromBody] bool type)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _commentVoteService.SetAsync(
            commentId,
            userId,
            type);
        return Created();
    }


    [Authorize]
    [HttpDelete("")]
    public async Task<ActionResult> DeleteVote(
        Guid commentId)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        await _commentVoteService.DeleteAsync(
            commentId,
            userId);
        return Created();
    }
}