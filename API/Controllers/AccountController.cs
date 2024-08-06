using API.DTOs.Account;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(JWTServices jwtService, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager) : BaseController
    {

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody]LoginDto model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Tên người dùng hoặc mật khẩu không đúng");

            if (user.EmailConfirmed == false) return Unauthorized("Vui lòng xác nhận email");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Tên người dùng hoặc mật khẩu không đúng");

            return CreateApplicationUserDto(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDto model)
        {
            if(await CheckEmailExisAsync(model.Email))
            {
                return BadRequest($"Địa chỉ email {model.Email} đã tồn tại. Hãy chọn một email khác");
            }

            var userToAdd = new AppUser
            {
                FirstName = model.FirstName.ToLower(),
                LastName = model.LastName.ToLower(),
                UserName = model.Email.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(userToAdd, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors.ToString());

            return Ok($"Tạo tài khoản thành công. Hãy đăng nhập!!!");
        }

        #region private helper method
        private UserDto CreateApplicationUserDto(AppUser user)
        {
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = jwtService.CreateJWT(user),
            };
        }

        private async Task<bool> CheckEmailExisAsync(string email)
        {
            return await userManager.Users.AnyAsync(u => u.Email == email.ToLower());
        }
        #endregion
    }
}
