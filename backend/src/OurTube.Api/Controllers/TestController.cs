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
}