using AutoMapper;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.UserAvatar;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>()
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.UserAvatar));

            CreateMap<UserAvatar, UserAvatarDto>();
        }

    }
}
