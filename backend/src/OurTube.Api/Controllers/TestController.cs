using Microsoft.AspNetCore.Mvc;

namespace OurTube.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        Console.WriteLine(User.Identity.IsAuthenticated.ToString());
        Console.WriteLine(Request.Cookies["SessionId"]);
        return Ok();
    }
    
    [HttpGet("/health")]
    public async Task<ActionResult> Health()
    {
        return Ok("I`m healthy");
    }
}