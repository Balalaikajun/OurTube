using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;
using System.Security.Claims;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
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
            var videos = await _recommendationService.GetRecommendationsAsync(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                Request.Cookies["SessionId"],
                limit,
                after,
                reload
                );
            var nextAfter = after + limit;
            
            return Ok(new PagedVideoDto()
            {
                Videos = videos,
                NextAfter = nextAfter
            });
        }
    }
}
