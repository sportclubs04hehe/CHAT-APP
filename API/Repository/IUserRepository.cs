using API.DTOs.User;
using API.Models;

namespace API.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(string id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<IEnumerable<MemberDto>> GetAllMemberAsync();
        Task<MemberDto?> GetMemberByUsernameAsync(string username);
    }
}
