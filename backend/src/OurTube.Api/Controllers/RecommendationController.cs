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
        private readonly RecomendationService _recommendationService;

        public RecommendationController(RecomendationService recommendationService)
        {
             _recommendationService = recommendationService;
        }
        
        [HttpGet]
        public ActionResult<PagedVideoDto> Get(
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            var videos = _recommendationService.GetVideos(
                limit,
                after,
                User.FindFirstValue(ClaimTypes.NameIdentifier));
            var nextAfter = after + limit;
            
            return Ok(new PagedVideoDto()
            {
                Videos = videos,
                NextAfter = nextAfter
            });
        }
    }
}
