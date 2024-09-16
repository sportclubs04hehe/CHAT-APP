using API.DTOs.Account;
using API.DTOs.Message;
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

            CreateMap<MemberUpdateDto, AppUser>();

            CreateMap<RegisterDto, AppUser>();

            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));

            CreateMap<Messages, MessageDto>()
               .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos
               .FirstOrDefault(x => x.IsMain)!.Url))
               .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos
               .FirstOrDefault(x => x.IsMain)!.Url))
               .ForMember(d => d.SenderKnowAs, o => o.MapFrom(s => s.Sender.KnowAs))
               .ForMember(d => d.RecipientKnowAs, o => o.MapFrom(s => s.Recipient.KnowAs));
        }
    }
}
