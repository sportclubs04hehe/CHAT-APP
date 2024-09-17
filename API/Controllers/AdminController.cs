using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager) : BaseController
    {
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("get-user-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            try
            {
                var users = await userManager.Users.ToListAsync();

                var usersWithRoles = new List<object>();

                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);

                    usersWithRoles.Add(new
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        Roles = roles
                    });
                }

                return Ok(usersWithRoles);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("get-photos")]
        public ActionResult GetPhotosForModerators()
        {
            return Ok("Chỉ quản trị viên và Người điều hành mới nhìn thấy api này");
        }

    }
}
