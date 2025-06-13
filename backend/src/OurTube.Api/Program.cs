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
var configuration = builder.Configuration;

// Infrastructure
FFmpeg.SetExecutablesPath(configuration["FFmpeg:ExecutablesPath"]);

// DB
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<DbContext, ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

services.AddMemoryCache();

// Auth
services.AddAuthorization();
services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme);

services.AddIdentity<IdentityUser, IdentityRole>(options =>
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
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Всегда ставить Secure
    options.Cookie.SameSite = SameSiteMode.None; // Говорим: можно в кросс-домен
    options.Cookie.Path = "/";
    options.Cookie.Name = "MyAppAuthCookie";
});

// Email
services.AddTransient<IEmailSender, EmailSender>();

// AutoMapper
services.AddAutoMapper(typeof(VideoProfile).Assembly);
services.AddAutoMapper(typeof(UserProfile).Assembly);
services.AddAutoMapper(typeof(UserProfile).Assembly);


// MediatR
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(PlaylistLikeHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(VideoCounterHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(LikedPlaylistHandler).Assembly);
});

// Application Services
services.AddScoped<VideoService>();
services.AddScoped<PlaylistService>();
services.AddScoped<VideoVoteService>();
services.AddScoped<UserService>();
services.AddScoped<CommentService>();
services.AddScoped<CommentVoteService>();
services.AddScoped<ViewService>();
services.AddScoped<IRecomendationService, RecommendationService>();
services.AddScoped<SubscriptionService>();
services.AddScoped<SearchService>();
services.AddScoped<TagService>();
services.AddScoped<UserAvatarService>();

// Infrastructure
services.AddScoped<IBlobService, MinioService>();
services.AddScoped<IVideoProcessor, FfmpegProcessor>();

// Other
services.AddScoped<LocalFilesService>();
services.AddScoped<VideoValidator>();

// CORS
services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
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
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OurTube API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите: Bearer {токен}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
services.AddControllers();

var app = builder.Build();

// Middleware


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.OAuthClientId("swagger-client-id");
        c.OAuthAppName("Swagger UI");
    });
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UniqueVisitorId>();

app.MapControllers();

app.MapGroup("/identity")
    .MapIdentityApi<IdentityUser>();

app.Run();