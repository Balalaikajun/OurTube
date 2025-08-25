using AutoMapper;
using OurTube.Application.Replies.UserAvatar;
using OurTube.Domain.Entities;
using UserAvatar = OurTube.Application.Replies.UserAvatar.UserAvatar;

namespace OurTube.Application.Mapping.AutoMapper;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<ApplicationUser, Replies.ApplicationUser.ApplicationUser>()
            .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.UserAvatar));

        CreateMap<Domain.Entities.UserAvatar, UserAvatar>();
    }
}