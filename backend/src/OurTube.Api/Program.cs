using Microsoft.EntityFrameworkCore;
using OurTube.Api;
using OurTube.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

Startup startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run(); 