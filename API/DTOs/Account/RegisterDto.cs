using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15,MinimumLength = 3, ErrorMessage = "Tên phải từ {2} đến {1} kí tự")]
        public required string FirstName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Tên đệm phải từ {2} đến {1} kí tự")]
        public required string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Biệt danh phải từ {2} đến {1} kí tự")]
        public required string KnowAs { get; set; } 

        [Required]
        public required string DateOfBirth { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ {2} đến {1} kí tự")]
        public required string Password { get; set; }
    }
}
