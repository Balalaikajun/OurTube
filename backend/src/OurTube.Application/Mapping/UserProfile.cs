using AutoMapper;
using OurTube.Application.DTOs;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserDTO>();

            CreateMap<UserAvatar, UserAvatarDTO>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket.Name));
        }

    }
}
