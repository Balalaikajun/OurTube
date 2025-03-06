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


        [HttpGet("{id}")]
        public  ActionResult<VideoGetDTO> Get(int id, VideoService videoService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            try
            {
                if (userId != null)
                    return Ok(videoService.GetVideoById(id, userId));
                else
                    return Ok(videoService.GetVideoById(id));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("Post")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post(
            [FromForm] VideoUploadDTO videoUploadDTO,
            [FromServices] VideoService videoService,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                await videoService.PostVideo(
                    videoUploadDTO,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    configuration["Minio:Endpoint"]);
                return Created();
            }
            catch(FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }



    }
}