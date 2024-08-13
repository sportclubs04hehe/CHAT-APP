using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Mật khẩu phải ít nhất {2} ký tự, và không quá {1} ký tự")]
        public string NewPassword { get; set; }
    }
}
