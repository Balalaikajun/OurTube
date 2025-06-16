using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.UserAvatar;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserAvatarService _userAvatarService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IUserAvatarService userAvatarService)
    {
        _userService = userService;
        _userAvatarService = userAvatarService;
    }

    [Authorize]
    [HttpPatch]
    public async Task<ActionResult> Patch(
        [FromBody] ApplicationUserPatchDto patchDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var result = await _userService.UpdateUserAsync(patchDto, userId);
            return Created(
                string.Empty,
                result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<ApplicationUserDto>> Get()
    {
        return await _userService.GetUserAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
    }

    [Authorize]
    [HttpPost("avatar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UserAvatarDto>> CreateOrUpdateAvatar([FromForm] UserAvatarPostDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _userAvatarService.CreateOrUpdateUserAvatarAsync(dto.Image, userId);
        return Created(string.Empty, result);
    }


    [Authorize]
    [HttpDelete("avatar")]
    public async Task<ActionResult> DeleteAvatar()
    {
        await _userAvatarService.DeleteUserAvatarAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

        return NoContent();
    }
}