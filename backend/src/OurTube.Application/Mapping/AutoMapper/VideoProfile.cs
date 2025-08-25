using AutoMapper;
using OurTube.Application.Replies.Video;
using OurTube.Domain.Entities;
using Video = OurTube.Domain.Entities.Video;

namespace OurTube.Application.Mapping.AutoMapper;

public class VideoProfile : Profile
{
    public VideoProfile()
    {
        CreateMap<Video, Replies.Video.Video>();

        CreateMap<Video, MinVideo>();

        CreateMap<VideoPlaylist, Replies.VideoPlaylist.VideoPlaylist>();

        CreateMap<VideoPreview, Replies.VideoPreview.VideoPreview>();
    }
}