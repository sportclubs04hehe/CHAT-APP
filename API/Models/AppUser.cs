using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
    public class AppUser : IdentityUser
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public required string KnowAs { get; set; }
        public required string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public List<Photo> Photos { get; set; } = [];

        public int GetAge()
        {
            return DateOfBirth.CalcuateAge();
        }
    }
}
