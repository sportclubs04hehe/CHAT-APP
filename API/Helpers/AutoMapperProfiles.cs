using API.DTOs.User;
using API.Extensions;
using API.Models;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // lấy ảnh đại diện làm Photo Url, ! sẽ là trả về null nếu rỗng thay vì 1 ngoại lệ (Exception)
            CreateMap<AppUser, MemberDto>()
                .ForMember(d => d.Age, o => o.MapFrom(m => m.DateOfBirth.CalcuateAge()))
                .ForMember(d => d.PhotoUrl, 
                o => o.MapFrom(m => m.Photos.FirstOrDefault(i => i.IsMain)!.Url)); 
            CreateMap<Photo, PhotoDto>();
        }
    }
}
