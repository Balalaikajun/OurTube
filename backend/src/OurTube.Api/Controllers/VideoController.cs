using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;

        public VideoController(VideoService videoService)
        {
             _videoService = videoService;
        }

        [HttpGet("{videoId:int}")]
        public ActionResult<VideoGetDto> Get(int videoId, VideoService videoService)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                return Ok(userId != null ? videoService.GetVideoById(videoId, userId) : videoService.GetVideoById(videoId));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post(
            [FromForm] VideoUploadDto videoUploadDto,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                await _videoService.PostVideo(
                    videoUploadDto,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    configuration["Minio:Endpoint"]);
                return Created();
            }
            catch (FormatException ex)
            {
                return BadRequest(ex.Message);
            }

        }



    }
}