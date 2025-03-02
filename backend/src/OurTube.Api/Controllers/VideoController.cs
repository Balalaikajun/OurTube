using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {


        [HttpGet("{id}")]
        public async Task<ActionResult<VideoDTO>> Get(int id, VideoService videoService)
        {
            //try
            //{
                return Ok(await videoService.GetVideoById(id));
            //}
            //catch (InvalidOperationException)
            //{
            //    return NotFound();
            //}
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