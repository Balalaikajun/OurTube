using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.Services;
using System.Security.Claims;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> Patch([FromBody] ApplicationUserPatchDTO patchDTO, UserService userService)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                await userService.UpdateUser(patchDTO, userId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
