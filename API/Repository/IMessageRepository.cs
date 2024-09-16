using API.Helpers;
using API.Models;
using API.DTOs.Message;

namespace API.Repository
{
    public interface IMessageRepository
    {
        // Thêm một tin nhắn mới vào cơ sở dữ liệu
        void AddMessage(Messages messages);

        // Xóa một tin nhắn khỏi cơ sở dữ liệu
        void DeleteMessage(Messages messages);

        // Lấy một tin nhắn dựa trên ID của nó
        Task<Messages?> GetMessagesAsync(Guid id);

        // Lấy danh sách tin nhắn cho một người dùng với phân trang
        Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);

        // Lấy chuỗi tin nhắn (thread) giữa hai người dùng
        Task<IEnumerable<MessageDto>> GetMessagesThreadAsync(string currentUsername, string recipientUsername);

        // Lưu tất cả thay đổi vào cơ sở dữ liệu (trả về true nếu lưu thành công)
        Task<bool> SaveAllAsync();
    }
}
