using AutoMapper;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.DTOs.Playlist;
using OurTube.Application.DTOs.PlaylistElement;
using OurTube.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Mapping
{
    public class PlaylistProfile:Profile
    {
        public PlaylistProfile()
        {
            CreateMap<Playlist, PlaylistGetDTO>();

            CreateMap<Playlist, PlaylistMinGetDTO>();

            CreateMap<PlaylistElement, PlaylistElementGetDTO>();
        }

    }
}
