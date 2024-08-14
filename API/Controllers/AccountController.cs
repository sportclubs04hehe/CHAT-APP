using API.DTOs.Account;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    public class AccountController(IJwtService jwtService,
        SignInManager<AppUser> signInManager,
        IEmailService emailService,
        IConfiguration config,
        UserManager<AppUser> userManager) : BaseController
    {

        #region Đăng nhập
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if (user == null) return Unauthorized("Tên người dùng hoặc mật khẩu không đúng");

            if (user.EmailConfirmed == false) return Unauthorized("Vui lòng xác nhận email");

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized("Tên người dùng hoặc mật khẩu không đúng");

            return CreateApplicationUserDto(user);
        }
        #endregion

        #region Làm mới token
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDto>> RefeshUserToken()
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user);
        }
        #endregion

        #region Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                if (await CheckEmailExisAsync(model.Email))
                {
                    return BadRequest($"Địa chỉ email {model.Email} đã tồn tại. Hãy chọn một tài khoản email khác");
                }

                var userToAdd = new AppUser
                {
                    FirstName = model.FirstName.ToLower(),
                    LastName = model.LastName.ToLower(),
                    UserName = model.Email.ToLower(),
                    Email = model.Email.ToLower(),
                };

                var result = await userManager.CreateAsync(userToAdd, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest("Đăng ký người dùng không thành công.");
                }

                if (await SendConfirmEmailAsync(userToAdd))
                {
                    return Ok(new JsonResult(new { title = "Tài khoản đã được tạo thành công", message = $"Chúng tôi đã gửi tin nhắn xác nhận tới email của bạn!" }));
                }
                else
                {
                    return StatusCode(500,"Tài khoản đã được tạo nhưng không gửi được email xác nhận. Vui lòng liên hệ với quản trị viên" );
                }

            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500,"Không gửi được email xác nhận. Vui lòng liên hệ với người quản trị.");
            }
            catch (Exception)
            {
                return StatusCode(500,"Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.");
            }
        }
        #endregion

        #region Xác nhận email
        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto emailDto)
        {
            try
            {
                // Find the user by email
                var user = await userManager.FindByEmailAsync(emailDto.Email);
                if (user == null)
                {
                    return Unauthorized("Địa chỉ email này chưa được đăng ký." );
                }

                // Check if the email is already confirmed
                if (user.EmailConfirmed)
                {
                    return BadRequest("Tài khoản email này đã được xác nhận trước đó. Vui lòng đăng nhập hoặc sử dụng email khác." );
                }

                // Decode the token
                string decodedToken;
                try
                {
                    var decodedTokenBytes = WebEncoders.Base64UrlDecode(emailDto.Token);
                    decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
                }
                catch (FormatException)
                {
                    return BadRequest("Mã xác nhận không hợp lệ. Vui lòng thử lại.");
                }
                catch (Exception ex)
                {
                    // Log the exception if needed
                    return StatusCode(500,"Đã xảy ra lỗi khi xử lý mã xác nhận.");
                }

                // Confirm the email
                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Xác nhận email thành công", message = "Địa chỉ email đã được xác nhận. Hãy đăng nhập." }));
                }

                // Handle case where email confirmation failed
                return BadRequest("Mã xác nhận không hợp lệ hoặc đã hết hạn. Vui lòng thử lại." );
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500,"Đã xảy ra lỗi khi xác nhận email. Vui lòng thử lại sau.");
            }
        }
        #endregion

        #region Gửi lại email
        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink([FromRoute] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email không hợp lệ.");
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Địa chỉ email này chưa được đăng ký." );
            }

            if (user.EmailConfirmed)
            {
                return BadRequest("Tài khoản email này đã được xác nhận trước đó. Vui lòng đăng nhập hoặc sử dụng email khác." );
            }

            try
            {
                // Attempt to resend the confirmation email
                var emailSent = await SendConfirmEmailAsync(user);
                if (emailSent)
                {
                    return Ok(new JsonResult(new { title = "Xác nhận liên kết đã gửi", message = "Hãy xác nhận địa chỉ email của bạn." }));
                }
                else
                {
                    // If email sending fails without an exception
                    return StatusCode(500, "Gửi email thất bại. Hãy liên hệ với quản trị viên." );
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình gửi email. Hãy liên hệ với quản trị viên.");
            }
        }
        #endregion

        #region Đặt lại mật khẩu bằng cách xác nhận email
        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotUsernameOrPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email không hợp lệ." );
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Địa chỉ email này chưa được đăng ký." );
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("Vui lòng xác nhận email của bạn trước." );
            }

            try
            {
                // Attempt to send the forgot username or password email
                var emailSent = await SendForgotUsernameOrPasswordEmail(user);
                if (emailSent)
                {
                    return Ok(new JsonResult(new { title = "Tên người dùng và mật khẩu đã được gửi", message = "Hãy kiểm tra email của bạn." }));
                }
                else
                {
                    // If email sending fails without an exception
                    return StatusCode(500,"Gửi email thất bại. Hãy liên hệ với quản trị viên." );
                }
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình gửi email. Hãy liên hệ với quản trị viên.");
            }
        }
        #endregion

        #region Đặt lại mật khẩu
        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("Yêu cầu không hợp lệ. Vui lòng kiểm tra lại thông tin nhập." );
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest( "Địa chỉ email này chưa được đăng ký." );
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("Vui lòng xác nhận email của bạn trước." );
            }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Đổi mật khẩu thành công", message = "Mật khẩu của bạn đã được thay đổi." }));
                }

                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { message = "Đổi mật khẩu thất bại.", errors = errorMessages });
            }
            catch (FormatException)
            {
                return BadRequest("Mã xác nhận không hợp lệ. Vui lòng thử lại." );
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "Đã xảy ra lỗi trong quá trình đổi mật khẩu. Hãy thử lại.");
            }
        }
        #endregion

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

        private async Task<bool> SendConfirmEmailAsync(AppUser appUser)
        {
            // tạo mã thông báo xác nhận email không đồng bộ
            var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ConfirmEmailPath"]}?token={token}&email={appUser.Email}";

            var body = @$"<p><b> Xin chào: {appUser.FirstName} {appUser.LastName} </b></p> 
                        <p> Vui lòng xác nhận email bằng cách nhấn vào link này </p>" +
                        $"<p><a href=\"{url}\"> Nhấn vào đây </a></p>" +
                        @$"<p>Cảm ơn</p> <br> {config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(appUser.Email, "Xác nhận email của bạn", body);

            return await emailService.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(AppUser appUser)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(appUser);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ResetPasswordPath"]}?token={token}&email={appUser.Email}";

            var body = @$"<p><b> Xin chào: {appUser.FirstName} {appUser.LastName} </b></p> 
                        <p> Tên đăng nhập: {appUser.UserName} </p>" +
                        $"Để đặt lại mật khẩu, hãy nhấn vào đường dẫn này." +
                        $"<p><a href=\"{url}\"> Nhấn vào đây </a></p>" +
                        @$"<p>Cảm ơn</p> <br> {config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(appUser.Email, "Đặt lại TÊN NGƯỜI DÙNG hoặc MẬT KHẨU", body);

            return await emailService.SendEmailAsync(emailSend);
        }
        #endregion
    }
}
