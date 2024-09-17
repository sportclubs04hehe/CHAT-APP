using API.Models;

namespace API.Services
{
    public interface IJwtService
    {
        Task<string> CreateJWT(AppUser user);
    }
}
