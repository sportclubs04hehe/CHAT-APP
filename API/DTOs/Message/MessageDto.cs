namespace API.DTOs.Message
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public required string SenderId { get; set; } // id người gửi
        public required string SenderUsername { get; set; } // Tài khoản người gửi
        public required string SenderKnowAs{ get; set; } // Biet Danh người gửi
        public required string SenderPhotoUrl { get; set; }
        public required string RecipientId { get; set; } // id người nhận
        public required string RecipientUsername { get; set; } // tên người dùng người nhận
        public required string RecipientKnowAs { get; set; } // Biet danh người nhận
        public required string RecipientPhotoUrl { get; set; }
        public required string Content { get; set; } // nội dung
        public DateTime? DateRead { get; set; } // ngày đọc
        public DateTime MessageSent { get; set; } // thời gian gửi
    }
}
