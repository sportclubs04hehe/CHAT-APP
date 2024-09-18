using API.Data;
using API.Services.Impl;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Repository;
using API.Repository.Impl;
using API.Helpers;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            // Add services to the container.

            services.AddControllers();

            services.AddDbContext<Context>(o =>
            {
                o.UseSqlServer(config.GetConnectionString("DefaultConnections"));
            });

            services.AddScoped<IJwtService, JWTService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoService, PhotoService>();

            #region Repository
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            #endregion

            #region LogUserActivity
            services.AddScoped<LogUserActivity>();
            #endregion

            #region AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            #endregion

            #region Clouldinary configure
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));

            #endregion

            #region SignalR
            services.AddSignalR();
            #endregion

            services.AddCors();

            // config error
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = activeContext =>
                {
                    var errors = activeContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();

                    var toReturn = new
                    {
                        Errors = errors
                    };

                    return new BadRequestObjectResult(toReturn);
                };
            });

            return services;

        }
    }
}
