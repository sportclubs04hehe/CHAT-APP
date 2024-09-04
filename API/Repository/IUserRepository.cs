using API.DTOs.User;
using API.Helpers;
using API.Models;

namespace API.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser?> GetUserByIdAsync(string id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
        Task<PagedList<MemberDto>> GetAllMemberAsync(UserParams userParams);
        Task<MemberDto?> GetMemberByUsernameAsync(string username);
    }
}
