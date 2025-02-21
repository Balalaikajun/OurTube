using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.DTOs;
using OurTube.Infrastructure.Data;

namespace OurTube.Application.Services
{
    public class VideoService
    {
        private ApplicationDbContext _context;
        private IMapper _mapper;
        public VideoService(ApplicationDbContext context, IMapper mapper) 
        {
            _context = context; 
            _mapper = mapper;
        }

        public VideoDTO GetVideoDTO(int id)
        {
            return _mapper.Map<VideoDTO>(
                _context.Videos
                .Include(v => v.VideoPreview)
                .Include(v => v.Playlists)
                .Include(v => v.ApplicationUser)
                .FirstAsync(v => v.Id == id));
        }
    }
}
