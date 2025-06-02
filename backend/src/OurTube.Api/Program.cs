using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using OurTube.Application.Handlers;
using OurTube.Application.Mapping;
using OurTube.Application.Services;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;
using OurTube.Infrastructure.Persistence;
using OurTube.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Infrastructure
Xabe.FFmpeg.FFmpeg.SetExecutablesPath(configuration["FFmpeg:ExecutablesPath"]);

// DB
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<DbContext, ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

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

// Email
services.AddTransient<IEmailSender, EmailSender>();

// AutoMapper
services.AddAutoMapper(typeof(VideoProfile).Assembly);
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
services.AddScoped<BaseRecomendationService>();
services.AddScoped<SubscriptionService>();
services.AddScoped<SearchService>();
services.AddScoped<TagService>();

// Repositories
services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
services.AddScoped<IPlaylistRepository, PlaylistRepository>();
services.AddScoped<IRepository<PlaylistElement>, Repository<PlaylistElement>>();

// Infrastructure
services.AddScoped<MinioService>();
services.AddScoped<FfmpegProcessor>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

// Other
services.AddScoped<LocalFilesService>();
services.AddScoped<VideoValidator>();

// CORS
services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Request size config
services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});
services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
});

// Controllers & Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    // Добавляем поддержку Bearer токена
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите токен в формате: Bearer {токен}"
    });

    // Обязательное указание схемы безопасности для всех контроллеров
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }});
});
services.AddControllers();

var app = builder.Build();

// Middleware
app.UseCors();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGroup("/identity")
    .MapIdentityApi<IdentityUser>();

app.Run();
