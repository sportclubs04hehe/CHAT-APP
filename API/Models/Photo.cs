using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Photos")]
    public class Photo
    {
        public Guid Id { get; set; }
        public required string Url { get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }

        //Navigation properties
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!; //  thuộc tính này sẽ được khởi tạo hợp lý trước khi nó được sử dụng.
    }
}