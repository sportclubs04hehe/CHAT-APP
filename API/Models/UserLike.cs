namespace API.Models
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } = null!;
        public string? SourceUserId { get; set; }
        public AppUser TargetUser { get; set; } = null!;
        public string? TargetUserId { get; set; }
    }
}
