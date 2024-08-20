using API.Data;
using API.Extensions;
using API.Middleware;
using API.Models;
using Microsoft.AspNetCore.Identity;

namespace API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationService(builder.Configuration);
            builder.Services.AddIdentityService(builder.Configuration);

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors(opt =>  
            {
                opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                .WithOrigins(builder.Configuration["JWT:ClientUrl"]);
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await Seed.SeedUsers(userManager);
            }
            catch(Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            app.Run();
        }
    }
}
