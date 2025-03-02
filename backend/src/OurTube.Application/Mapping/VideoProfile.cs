using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OurTube.Domain.Entities;
using OurTube.Application.DTOs;

namespace OurTube.Application.Mapping
{
    public class VideoProfile:Profile
    {
        public VideoProfile()
        {
            CreateMap<Video, VideoDTO>()
                .ForMember(dest => dest.Preview, opt => opt.MapFrom(src => src.VideoPreview))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.ApplicationUser)); ;

            CreateMap<VideoPlaylist, VideoPlaylistDTO>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket.Name));

            CreateMap<VideoPreview, VideoPreviewDTO>()
                .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.Bucket.Name));
        }

    }
}
