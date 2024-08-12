using API.Models;

namespace API.Services
{
    public interface IJwtService
    {
        string CreateJWT(AppUser user);
    }
}
