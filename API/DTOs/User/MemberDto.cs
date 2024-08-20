using API.Models;

namespace API.DTOs.User
{
    public class MemberDto
    {
        public string Id { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; }
        public int Age { get; set; }
        public string? PhotoUrl { get; set; }
        public string? KnowAs { get; set; }
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<PhotoDto> Photos { get; set; } = [];
    }
}
