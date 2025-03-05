using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Views;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddVideo(
            [FromBody] ViewPostDTO postDTO,
            [FromServices] ViewService viewService)
        {
            try
            {
                await viewService.AddVideo(
                    postDTO.VideoId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    postDTO.EndTime);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{videoId}")]
        public async Task<ActionResult> RemoveVideo(
            int videoId,
            [FromServices] ViewService viewService)
        {
            try
            {
                await viewService.RemoveVideo(
                    videoId,
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete()]
        public async Task<ActionResult> ClearHistory(
            [FromServices] ViewService viewService)
        {
            try
            {
                await viewService.ClearHistory(
                    User.FindFirstValue(ClaimTypes.NameIdentifier));

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> Get(
            [FromServices] ViewService viewService,
            [FromQuery] int limit = 10,
            [FromQuery] int after = 0)
        {
            try
            {
                List<ViewGetDTO> history = viewService.GetWithLimit(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                limit,
                after);
                int nextAfter = after + limit;


                return Ok(new { history, nextAfter });
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);

            }

        }
    }
}
