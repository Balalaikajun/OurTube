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
        public ActionResult<List<VideoMinGetDto>> Get(
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            return Ok(_recommendationService.GetVideos(
                limit,
                after,
                User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
    }
}
