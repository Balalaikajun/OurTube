using AutoMapper;
using OurTube.Application.DTOs.Video;
using OurTube.Application.DTOs.VideoPlaylist;
using OurTube.Application.DTOs.VideoPreview;
using OurTube.Domain.Entities;

namespace OurTube.Application.Mapping
{
    public class VideoProfile : Profile
    {
        public VideoProfile()
        {
            CreateMap<Video, VideoGetDTO>();

            CreateMap<Video, VideoMinGetDTO>();

            CreateMap<VideoPlaylist, VideoPlaylistDTO>();

            CreateMap<VideoPreview, VideoPreviewDTO>();
        }

    }
}
