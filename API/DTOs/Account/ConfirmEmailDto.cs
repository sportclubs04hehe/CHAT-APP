using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Account
{
    public class ConfirmEmailDto
    {
        [Required] public string Token { get; set; }

        [Required]
        [RegularExpression("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$", ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string Email { get; set; }
    }
}
