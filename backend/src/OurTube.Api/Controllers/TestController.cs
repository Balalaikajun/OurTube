using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OurTube.Infrastructure.Other;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController: ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> Get([FromServices]MinioService minioService)
    {
        await minioService.UploadFile( @"C:\Users\user\Pictures\Фоновые изображения рабочего стола\1. Черный кот с рыбкой.jpg", "sdsdf.jpg", "videos");
        
        return Ok();
    }
}
    