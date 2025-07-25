using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;

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
    public async Task<ActionResult<PagedVideoDto>> Get(
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);
        
        var result = await _recommendationService.GetRecommendationsAsync(
            userId,
            sessionId,
            limit,
            after,
            reload
        );

        return Ok(result);
    }
    
    [HttpGet("video/{videoId:guid}")]
    public async Task<ActionResult<PagedVideoDto>> GetForVideo(
        Guid videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);
        
        var result = await _recommendationService.GetRecommendationsForVideoAsync(
            videoId,
            userId,
            sessionId,
            limit,
            after,
            reload
        );
        
        return Ok(result);
    }
}