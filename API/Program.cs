using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<Context>(o =>
            {
                o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnections"));
            });

            builder.Services.AddScoped<JWTServices>();

            // defining our IndentityCore Service
            builder.Services.AddIdentityCore<AppUser>(o =>
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
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        // khoa ky cua ben phat hanh dua tren JWT:Key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                        // nguoi phat hanh o day la url cua du an dang su dung
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        // ValidateIssuer(bat ki ai phat hanh JWT)
                        ValidateIssuer = true,
                        // khong xac thuc ValidateAudience (angular side)
                        ValidateAudience = false,
                    };
                });

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
