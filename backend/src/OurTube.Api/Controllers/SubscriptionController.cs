using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Subscribe (string userToId, SubscriptionService subscriptionService)
        {
            try
            {
                await subscriptionService.Subscribe(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userToId);

                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> UnSubscribe(string userToId, SubscriptionService subscriptionService)
        {
            try
            {
                await subscriptionService.UnSubscribe(
                    User.FindFirstValue(ClaimTypes.NameIdentifier),
                    userToId);

                return Created();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
