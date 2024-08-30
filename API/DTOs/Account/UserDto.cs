namespace API.DTOs.Account
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string JWT { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
