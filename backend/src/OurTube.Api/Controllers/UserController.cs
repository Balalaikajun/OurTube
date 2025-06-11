using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.UserAvatar;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly UserAvatarService _userAvatarService;

    public UserController(UserService userService, UserAvatarService userAvatarService)
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
            await _userService.UpdateUserAsync(patchDto, userId);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
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
        await _userAvatarService.DeleteUserAvatarAsync( User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        return NoContent();
    }
}