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
        var result = await _recommendationService.GetRecommendationsAsync(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            Request.Cookies["SessionId"],
            limit,
            after,
            reload
        );

        return Ok(result);
    }
    
    [HttpGet("video/{videoId:int}")]
    public async Task<ActionResult<PagedVideoDto>> GetForVideo(
        int videoId,
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = false)
    {
        var result = await _recommendationService.GetRecommendationsForVideoAsync(
            videoId,
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            Request.Cookies["SessionId"],
            limit,
            after,
            reload
        );
        
        return Ok(result);
    }
}