using HostingPrototype.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;

namespace OurTube.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpPost("user")]
        public IActionResult ParseUser([FromBody]string id, ApplicationDbContext context)
        {
            context.ApplicationUsers.Add(new ApplicationUser { Id = id });
            context.SaveChanges();
            return Ok();
        }

        [HttpPost("video")]
        public IActionResult ParseVideo([FromBody] string userid, ApplicationDbContext context)
        {
            Video video = new Video
            {
                Title = "asd",
                Description = "sadf",
                ApplicationUser = context.ApplicationUsers.Find("f2dac030-9c50-407b-aedb-87e2459402f6")
            };
            context.Videos.Add(video);
            context.SaveChanges();
            return Ok();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("mail")]
        public async Task<IActionResult> TestMail([FromServices] IEmailSender emailSender)
        {
            await emailSender.SendEmailAsync("andrejkluev774@gmail.com", "Привет друг, не бойся", "<h1>Добро пожаловать!</h1>");
            return Ok();
        }

        [HttpGet("videoUpload")]
        public async Task<IActionResult> VideoUploadTest([FromServices] MinioService minioService)
        {
            await minioService.UploadFile(@"C:\Users\user\Downloads\mark2.jpg", "test/1.jpg", "carimages");
            return Ok();
        }

    }
}
