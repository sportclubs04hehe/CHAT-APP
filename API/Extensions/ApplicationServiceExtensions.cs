using API.Data;
using API.Services.Impl;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Repository;
using API.Repository.Impl;

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

            #region Repository
            services.AddScoped<IUserRepository, UserRepository>();
            #endregion

            #region AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
