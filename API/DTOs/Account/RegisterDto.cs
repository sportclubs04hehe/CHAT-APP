﻿using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(15,MinimumLength = 3, ErrorMessage = "Tên phải từ {2} đến {1} kí tự")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Tên đệm phải từ {2} đến {1} kí tự")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Biệt danh phải từ {2} đến {1} kí tự")]
        public string KnowAs { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Thành phố phải từ {2} đến {1} kí tự")]
        public string City { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Quốc gia phải từ {2} đến {1} kí tự")]
        public string Country { get; set; }

        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ {2} đến {1} kí tự")]
        public string Password { get; set; }
    }
}
