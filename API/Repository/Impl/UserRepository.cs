using API.Data;
using API.DTOs.User;
using API.Helpers;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Impl
{
    public class UserRepository(Context context,
        IMapper mapper) : IUserRepository
    {
        public async Task<PagedList<MemberDto>> GetAllMemberAsync(UserParams userParams)
        {
            //var query = context.Users.AsQueryable();

            //query = query.Where(x => x.UserName != userParams.CurrentUsername);

            //if (userParams.Gender != null)
            //{
            //    query = query.Where(x => x.Gender == userParams.Gender);
            //}

            //var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            //var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            //query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            //query = userParams.OrderBy switch
            //{
            //    "created" => query.OrderByDescending(x => x.DateCreated),
            //    _ => query.OrderByDescending(x => x.LastActive)
            //};

            //return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider),
            //    userParams.PageNumber, userParams.PageSize);

            var query = context.Users
                 .OrderBy(u => u.UserName)
                 .ProjectTo<MemberDto>(mapper.ConfigurationProvider);

            return await PagedList<MemberDto>.CreateAsync(query, userParams.PageNumber, userParams.PageSize);
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
