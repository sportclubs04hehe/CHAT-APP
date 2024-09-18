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

        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, string roles)
        {
            try
            {
                if(string.IsNullOrEmpty(roles))
                {
                    return BadRequest("Bạn phải chọn ít nhất 1 quyền ");
                }

                var selectedRoles = roles.Split(',').ToArray();

                var user = await userManager.FindByNameAsync(username);

                if (user == null) {
                    return NotFound("User not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);

                var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

                if(!result.Succeeded)
                {
                    return BadRequest("Có vấn đề khi thêm quyền");
                }

                result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

                if (!result.Succeeded)
                {
                    return BadRequest("Có vấn đề khi xóa quyền");
                }

                return Ok(await userManager.GetRolesAsync(user));
            }
            catch (Exception ex)
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
