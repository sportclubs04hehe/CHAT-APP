using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions {  PropertyNameCaseInsensitive = true };

            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            if (users == null) return;

            // Tạo các quyền
            if (!await roleManager.RoleExistsAsync("ADMIN"))
            {
                await roleManager.CreateAsync(new IdentityRole("ADMIN"));
            }
            if (!await roleManager.RoleExistsAsync("MODERATOR"))
            {
                await roleManager.CreateAsync(new IdentityRole("MODERATOR"));
            }
            if (!await roleManager.RoleExistsAsync("MEMBER"))
            {
                await roleManager.CreateAsync(new IdentityRole("MEMBER"));
            }

            foreach (var user in users)
            {
                user.UserName = user.Email.ToLower();
                user.EmailConfirmed = true;

                

                // The password should be handled by the UserManager to ensure proper hashing
                await userManager.CreateAsync(user, "123456");

                // Gán quyền MEMBER cho người dùng
                await userManager.AddToRoleAsync(user, "MEMBER");
            }

            var admin = new AppUser
            {
                FirstName = "Admin",
                LastName = "System",
                UserName = "admintraction@gmail.com",
                Email = "admintraction@gmail.com",
                KnowAs = "Admin",
                Gender = "male",
                EmailConfirmed = true,
            };

            await userManager.CreateAsync(admin, "123456");
            await userManager.AddToRolesAsync(admin, ["ADMIN", "MODERATOR"]);
        }
    }
}
