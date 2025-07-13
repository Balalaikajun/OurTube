using Microsoft.AspNetCore.Mvc;

namespace OurTube.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController:ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("I`m healthy");
    }
}