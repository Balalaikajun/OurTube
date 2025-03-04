using AutoMapper;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class PlaylistService
    {
        private ApplicationDbContext _dbContext;
        private IMapper _mapper;

        public PlaylistService(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }


    }
}
