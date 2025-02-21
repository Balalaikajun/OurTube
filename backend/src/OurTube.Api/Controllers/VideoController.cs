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
                return Ok(videoService.GetVideoDTO(id));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
        //[HttpPost]
        //public ActionResult Post(
        //    [FromForm] VideoPostDTO videoPostDTO,
        //    [FromForm] IFormFile videoFile,
        //    [FromForm] IFormFile previewFile)
        //{

        //}



    }
}