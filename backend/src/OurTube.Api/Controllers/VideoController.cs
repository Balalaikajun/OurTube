using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {


        [HttpGet("{id}")]
        public ActionResult<VideoDTO> Get(int id, VideoService videoService)
        {
            try
            {
                return Ok(videoService.GetVideoById(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost("Post")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Post(
            [FromForm] VideoUploadDTO videoUploadDTO,
            [FromServices] VideoService videoService,
            [FromServices] IConfiguration configuration)
        {
            try
            {
                await videoService.PostVideo(videoUploadDTO, "1", configuration["Minio:Endpoint"]);
                return Created();
            }
            catch(FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }



    }
}