using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace OurTube.Api.Controllers;

[Route("identity")]
[ApiController]
public class IdentityController: ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public IdentityController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }
    
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        _signInManager.SignOutAsync();
        
        return Ok();
    }
}