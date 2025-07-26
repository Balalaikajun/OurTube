using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Recommendation;

namespace OurTube.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class RecommendationController : ControllerBase
{
    private readonly IRecomendationService _recommendationService;

    public RecommendationController(IRecomendationService recommendationService)
    {
        _recommendationService = recommendationService;
    }

    [HttpGet]
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);
        
        var result = await _recommendationService.GetRecommendationsAsync(new GetRecommendationsRequest()
        {
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });

        return Ok(result);
    }
    
    [HttpGet("video/{videoId:guid}")]
    public async Task<ActionResult<ListReply<MinVideo>>> GetForVideo(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);
        
        var result = await _recommendationService.GetRecommendationsForVideoAsync(new GetRecommendationsForVideoRequest()
        {
            VideoId = videoId,
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });
        
        return Ok(result);
    }
}