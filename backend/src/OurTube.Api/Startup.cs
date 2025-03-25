using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Mapping;
using OurTube.Application.Services;
using OurTube.Application.Validators;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Infrastructure.Data;
using OurTube.Infrastructure.Other;
using OurTube.Infrastructure.Persistence;
using OurTube.Infrastructure.Persistence.Repositories;
using System.Reflection;
using OurTube.Application.Handlers;


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

            services.AddDbContext<DbContext, ApplicationDbContext>(options =>
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

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(PlaylistLikeHandler).Assembly); 
                cfg.RegisterServicesFromAssembly(typeof(VideoCounterHandler).Assembly);
                cfg.RegisterServicesFromAssembly(typeof(LikedPlaylistHandler).Assembly);
                
            });

            // Services
            services.AddScoped<VideoService>();
            services.AddScoped<PlaylistService>();
            services.AddScoped<VideoVoteService>();
            services.AddScoped<UserService>();
            services.AddScoped<CommentService>();
            services.AddScoped<CommentVoteService>();
            services.AddScoped<ViewService>();
            services.AddScoped<BaseRecomendationService>();
            services.AddScoped<SubscriptionService>();
            services.AddScoped < SearchService>();
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
