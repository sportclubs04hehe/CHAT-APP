using API.DTOs.User;
using API.Helpers;
using API.Models;

namespace API.Repository
{
    public interface ILikesRepository
    {
        /// Lấy thông tin về việc người dùng này đã thích người dùng khác chưa
        Task<UserLike?> GetUserLike(string sourceUserId, string targetUserId);
        /// Lấy danh sách các thành viên được người dùng thích hoặc đã thích người dùng (dựa trên `predicate`)
        Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams);
        /// Lấy danh sách ID của những người dùng mà người dùng hiện tại đã thích
        Task<IEnumerable<string>> GetCurrentUserLikeIds(string currentUserId);
        /// Xóa một bản ghi "like" khỏi cơ sở dữ liệu
        void DeleteLike(UserLike like);
        /// Thêm một bản ghi "like" mới vào cơ sở dữ liệu
        void AddLike(UserLike like);
        /// Lưu các thay đổi vào cơ sở dữ liệu
        Task<bool> SaveChange();
    }
}
