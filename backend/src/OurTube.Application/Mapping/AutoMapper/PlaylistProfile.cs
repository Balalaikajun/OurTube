using AutoMapper;
using OurTube.Domain.Entities;
using PlaylistElement = OurTube.Application.Replies.PlaylistElement.PlaylistElement;

namespace OurTube.Application.Mapping.AutoMapper;

public class PlaylistProfile : Profile
{
    public PlaylistProfile()
    {
        CreateMap<Playlist, Replies.Playlist.Playlist>();

        CreateMap<Domain.Entities.PlaylistElement, PlaylistElement>();
    }
}