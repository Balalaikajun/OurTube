using Microsoft.AspNetCore.Http;
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
        [HttpGet]
        public ActionResult<List<VideoMinGetDTO>> Get(
            [FromServices] RecomendationService recomendationService,
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            return Ok(recomendationService.GetVideos(
                limit,
                after,
                User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
    }
}
