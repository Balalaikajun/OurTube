using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OurTube.Infrastructure.Other;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get()
    {
        Console.WriteLine(User.Identity.IsAuthenticated.ToString());
        Console.WriteLine(Request.Cookies["SessionId"]);
        return Ok();
    }
}
    