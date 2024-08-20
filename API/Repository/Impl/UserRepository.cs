using API.Data;
using API.DTOs.User;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Impl
{
    public class UserRepository(Context context,
        IMapper mapper) : IUserRepository
    {
        public async Task<IEnumerable<MemberDto>> GetAllMemberAsync()
        {
            return await context.Users
               .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
        {
            return await context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<MemberDto?> GetMemberByUsernameAsync(string username)
        {
            return await context.Users
                .Where(u => u.UserName == username.ToLower())
                .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<AppUser?> GetUserByIdAsync(string id)
        {
            return await context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(u => u.UserName == username.ToLower());
        }

    }
}
