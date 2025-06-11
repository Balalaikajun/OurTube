using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/Video/Comment/{commentId:int}/vote")]
[ApiController]
public class CommentVoteController : ControllerBase
{
    private readonly CommentVoteService _commentVoteService;

    public CommentVoteController(CommentVoteService commentVoteService)
    {
        _commentVoteService = commentVoteService;
    }

    [Authorize]
    [HttpPost("")]
    public async Task<ActionResult> PostVote(
        int commentId,
        [FromBody] bool type)
    {
        try
        {
            await _commentVoteService.SetAsync(
                commentId,
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                type);
            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize]
    [HttpDelete("")]
    public async Task<ActionResult> DeleteVote(
        int commentId)
    {
        try
        {
            await _commentVoteService.DeleteAsync(
                commentId,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            return Created();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}