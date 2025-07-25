using System.Reflection;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OurTube.Api.Middlewares;
using OurTube.Application.Handlers;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping;
using OurTube.Application.Services;
using OurTube.Application.Validators;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;
using Xabe.FFmpeg;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
builder.Configuration.AddEnvironmentVariables();
var configuration = builder.Configuration;

// Infrastructure
var ffmpegPath = configuration["FFmpeg:ExecutablesPath"];
if (!File.Exists(ffmpegPath + "/ffmpeg.exe"))
{
    if(configuration.GetValue<bool>("FFmpeg:AutoDownload"))
    {
        Console.WriteLine("Исполняемые файлы ffmpeg не найдены");
        await FfmpegProcessor.DownloadAndExtractFFmpegAsync(ffmpegPath);
    }
    else
    {
        throw new FileNotFoundException("ffmpeg.exe not found");
    }
}
FFmpeg.SetExecutablesPath(configuration["FFmpeg:ExecutablesPath"]);


// DB
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<DbContext, ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

services.AddMemoryCache();

// Auth
services.AddAuthorization();
services.AddAuthentication();

services.AddIdentity<OurTube.Domain.Entities.IdentityUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Tokens.AuthenticatorIssuer = null;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserManager<ApplicationUserManager>()
    .AddApiEndpoints();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = PathString.Empty;
    options.AccessDeniedPath = PathString.Empty;
    
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.Remove("Location");
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.Headers.Remove("Location");
        return Task.CompletedTask;
    };

    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Всегда ставить Secure
    options.Cookie.SameSite = SameSiteMode.None; // Говорим: можно в кросс-домен
    options.Cookie.Path = "/";
    options.Cookie.Name = "MyAppAuthCookie";
});

// Email
services.AddTransient<IEmailSender, EmailSender>();

// AutoMapper
services.AddAutoMapper(cfg => { }, typeof(VideoProfile).Assembly);


// MediatR
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PlaylistLikeHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(VideoCounterHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LikedPlaylistHandler).Assembly);
});

// Application Services
services.AddScoped<IVideoService, VideoService>();
services.AddScoped<IPlaylistCrudService, PlaylistService>();
services.AddScoped<IPlaylistQueryService, PlaylistService>();
services.AddScoped<IVideoVoteService, VideoVoteService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ICommentCrudService, CommentService>();
services.AddScoped<ICommentRecommendationService, CommentService>();
services.AddScoped<ICommentVoteService, CommentVoteService>();
services.AddScoped<IViewService, ViewService>();
services.AddScoped<IRecomendationService, RecommendationService>();
services.AddScoped<ISubscriptionService, SubscriptionService>();
services.AddScoped<ISearchService, SearchService>();
services.AddScoped<ITagService, TagService>();
services.AddScoped<IUserAvatarService, UserAvatarService>();

// Infrastructure
services.AddScoped<IStorageClient, MinioClient>();
services.AddScoped<IVideoProcessor, FfmpegProcessor>();

// Other
services.AddScoped<VideoValidator>();

// CORS
var originsEnv = configuration["Cors:AllowedOrigins"];
var allowedOrigins = originsEnv.Split(';', StringSplitOptions.RemoveEmptyEntries);
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Request size config
services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = int.MaxValue; });
services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = long.MaxValue; });
services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = long.MaxValue; });

// Controllers & Swagger
services.AddEndpointsApiExplorer();
if (builder.Environment.IsDevelopment())
{
    services.AddSwaggerGen(c =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        
        var xmlDtoPath = Path.Combine(AppContext.BaseDirectory, "OurTube.Application.xml");
        c.IncludeXmlComments(xmlDtoPath);
        
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "OurTube API", Version = "v1" });
    });
}

services.AddControllers();

// DataProtection directory
var keysPath = builder.Configuration["DataProtection:KeysPath"]
               ?? Path.Combine(builder.Environment.ContentRootPath, "DataProtection.Keys");

Directory.CreateDirectory(keysPath);

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var storageClient = scope.ServiceProvider.GetRequiredService<IStorageClient>();
    var videoBucket = configuration["MinIO:VideoBucket"];
    var userBucket = configuration["MinIO:UserBucket"];
    await storageClient.EnsureBucketsExistAsync(videoBucket, userBucket);
}

// Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.OAuthClientId("swagger-client-id");
        c.OAuthAppName("Swagger UI");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UniqueVisitorId>();

app.MapControllers();

app.MapGroup("/identity")
    .MapIdentityApi<IdentityUser>()
    .WithTags("Identity");

app.Run();