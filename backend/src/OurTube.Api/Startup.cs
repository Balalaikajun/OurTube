using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Infrastructure;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;
using OurTube.Application.Mapping;
using OurTube.Application.Services;
using OurTube.Application.Validators;


namespace OurTube.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

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

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddAutoMapper(typeof(VideoProfile).Assembly);
            services.AddAutoMapper(typeof(UserProfile).Assembly);

            services.AddScoped<VideoService>();
            services.AddScoped<PlaylistService>();
            services.AddScoped<VideoVoteService>();
            services.AddScoped<UserService>();
            services.AddScoped<CommentService>();
            services.AddScoped<CommentVoteService>();
            services.AddScoped<ViewService>();
            services.AddScoped<MinioService>();
            services.AddScoped<FfmpegProcessor>();

            services.AddScoped<LocalFilesService>();
            services.AddScoped<VideoValidator>();


            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            //Убираем ограничение по размеру запроса
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

            

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers();
            
            // Добавление аутентификации по токену
            //services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.UseEndpoints(s =>
            {
                s.MapControllers();
                s.MapGroup("/identity")
                    .MapIdentityApi<IdentityUser>();
            });
        }
    }
}
