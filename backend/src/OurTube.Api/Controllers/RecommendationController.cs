using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController : ControllerBase
    {
        private readonly BaseRecomendationService _recommendationService;

        public RecommendationController(BaseRecomendationService recommendationService)
        {
             _recommendationService = recommendationService;
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedVideoDto>> GetAsync(
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IEnumerable<VideoMinGetDto> videos;
            
            if (userId == null)
            {
                videos = await _recommendationService.GetRecomendationsAsync(
                    limit,
                    after
                );
            }
            else
            {
                videos = await _recommendationService.GetRecomendationsAsync(
                    limit,
                    after,
                    userId
                );
            }
            
            var nextAfter = after + limit;
            
            return Ok(new PagedVideoDto()
            {
                Videos = videos,
                NextAfter = nextAfter
            });
        }
    }
}
