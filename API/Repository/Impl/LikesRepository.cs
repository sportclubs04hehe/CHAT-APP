using API.Data;
using API.DTOs.User;
using API.Helpers;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Impl
{
    public class LikesRepository(Context context,
        IMapper mapper) : ILikesRepository
    {
        public void AddLike(UserLike like)
        {
            context.Likes.Add(like);
        }

        public void DeleteLike(UserLike like)
        {
            context.Likes.Remove(like);
        }

        /// Lấy danh sách ID của người dùng mà người dùng hiện tại đã thích
        public async Task<IEnumerable<string>> GetCurrentUserLikeIds(string currentUserId)
        {
            if (string.IsNullOrEmpty(currentUserId))
                return Enumerable.Empty<string>();

            return await context.Likes
                .Where(x => x.SourceUserId == currentUserId)
                .Select(x => x.TargetUserId)
                .ToListAsync(); 
        }

        public async Task<UserLike?> GetUserLike(string sourceUserId, string targetUserId)
        {
            return await context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        /// Lấy danh sách người dùng đã thích hoặc được thích dựa trên `predicate`
        /// Nếu `liked`: trả về những người dùng mà người dùng hiện tại đã thích
        /// Nếu `likedBy`: trả về những người dùng đã thích người dùng hiện tại
        /// Nếu không có giá trị `predicate`, trả về những người dùng đã thích và được thích bởi người dùng hiện tại
        public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
        {
            var likes = context.Likes.AsQueryable();
            IQueryable<MemberDto> query;

            switch (likesParams.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(x => x.SourceUserId == likesParams.UserId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .OrderBy(u => u.Id); 

                    break;

                case "likedBy":
                    query = likes
                        .Where(x => x.TargetUserId == likesParams.UserId)
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                        .OrderBy(u => u.Id); 

                    break;

                default:
                    // Lấy ID của các người dùng mà user đã thích và người dùng thích user
                    var likedUserIds = likes
                        .Where(x => x.SourceUserId == likesParams.UserId)
                        .Select(x => x.TargetUser.Id);

                    var likedByUserIds = likes
                        .Where(x => x.TargetUserId == likesParams.UserId)
                        .Select(x => x.SourceUser.Id);

                    // Kết hợp các ID và loại bỏ các bản ghi trùng lặp
                    var combinedUserIds = likedUserIds
                        .Union(likedByUserIds);  // Union giúp loại bỏ trùng lặp

                    // Lấy các đối tượng người dùng từ danh sách ID đã kết hợp
                    query = context.Users
                        .Where(u => combinedUserIds.Contains(u.Id))
                        .OrderBy(u => u.Id)
                        .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                    break;
            }

            return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
        }

        public async Task<int> GetLikedCountAsync(string userId)
        {
            return await context.Likes
                .Where(ul => ul.SourceUserId == userId)
                .CountAsync();
        }

        public async Task<int> GetLikedByCountAsync(string userId)
        {
            return await context.Likes
                .Where(ul => ul.TargetUserId == userId)
                .CountAsync();
        }

        public async Task<bool> SaveChange()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
