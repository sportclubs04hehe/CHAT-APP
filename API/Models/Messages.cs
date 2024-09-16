namespace API.Models
{
    public class Messages
    {
        public Guid Id { get; set; }
        public string? SenderId { get; set; } // id người gửi
        public required string SenderUsername { get; set; } // Tài khoản người gửi
        public AppUser Sender { get; set; } = null!;// người gửi
        public string? RecipientId { get; set; } // id người nhận
        public required string RecipientUsername { get; set; } // Tài khoản người nhận
        public AppUser Recipient { get; set; } = null!;// người nhận
        public required string Content { get; set; } // nội dung
        public DateTime? DateRead { get; set; } // ngày đọc
        public DateTime MessageSent { get; set; } = DateTime.Now; // thời gian gửi
        public bool SenderDeleted { get; set; } // xóa tin nhắn 
        public bool RecipientDeleted { get; set; } // Người nhận đã bị xóa
    }
}
