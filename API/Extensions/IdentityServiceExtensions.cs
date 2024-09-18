using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration config)
        {

            // defining our IndentityCore Service
            services.AddIdentityCore<AppUser>(o =>
            {
                // password configuration
                o.Password.RequiredLength = 6;
                o.Password.RequireDigit = false; // != number
                o.Password.RequireLowercase = false; // chu thuong = false
                o.Password.RequireUppercase = false; // chu hoa = false
                o.Password.RequireNonAlphanumeric = false; // khong phai chu va so = false

                // for email confirmation
                o.SignIn.RequireConfirmedEmail = true; // email xac nhan = true
            })
                .AddRoles<IdentityRole>() // co the them roles
                .AddRoleManager<RoleManager<IdentityRole>>() // co the s/d trinh quan li vai tro (Rolemanager)
                .AddEntityFrameworkStores<Context>() // cung cap Context 
                .AddSignInManager<SignInManager<AppUser>>() // s/d trinh quan li login 
                .AddUserManager<UserManager<AppUser>>() // tao nguoi dung s/d (UserManager)
                .AddDefaultTokenProviders(); // tao ma thong bao xac nhan email

            // co the uy quyen nguoi dung su dung jwt
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        // khoa ky cua ben phat hanh dua tren JWT:Key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"])),
                        // nguoi phat hanh o day la url cua du an dang su dung
                        ValidIssuer = config["JWT:Issuer"],
                        // ValidateIssuer(bat ki ai phat hanh JWT)
                        ValidateIssuer = true,
                        // khong xac thuc ValidateAudience (angular side)
                        ValidateAudience = false,
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;    
                        }
                    };
                });

            services.AddAuthorizationBuilder()
                .AddPolicy("RequiredAdminRole", policy => policy.RequireRole("ADMIN"))
                .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("ADMIN", "MODERATOR"));

            return services;
        }
    }
}
