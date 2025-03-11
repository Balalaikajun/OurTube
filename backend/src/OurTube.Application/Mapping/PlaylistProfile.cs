using AutoMapper;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping
{
    public class PlaylistProfile : Profile
    {
        public PlaylistProfile()
        {
            CreateMap<Playlist, PlaylistGetDTO>();

            CreateMap<Playlist, PlaylistMinGetDTO>();

            CreateMap<PlaylistElement, PlaylistElementGetDTO>();
        }

    }
}
