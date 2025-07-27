using Microsoft.AspNetCore.Mvc;

namespace OurTube.Api.Controllers;

/// <summary>
///     Контроллер проверки состояния сервера (health check).
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    ///     Проверить, что сервер работает корректно.
    /// </summary>
    /// <returns>Строка с подтверждением работоспособности.</returns>
    /// <response code="200">Сервер работает и вернул сообщение о статусе.</response>
    [HttpGet]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public ActionResult<string> Get()
    {
        return Ok("I'm healthy");
    }
}