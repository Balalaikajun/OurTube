using AutoMapper;
using OurTube.Application.DTOs.Views;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping
{
    public class ViewProfile : Profile
    {
        public ViewProfile()
        {
            CreateMap<VideoView, ViewGetDto>();
        }

    }
}
