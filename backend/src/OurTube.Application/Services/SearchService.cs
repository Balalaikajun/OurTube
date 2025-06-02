using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Application.DTOs;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;

namespace OurTube.Application.Services;

public class SearchService
{
    private IApplicationDbContext _dbContext;
    private IMapper _mapper;

    public SearchService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VideoMinGetDto>> SearchVideos(string searchQuery)
    {
        return await _dbContext.Videos
            .Where(v =>  EF.Functions.ToTsVector(v.Title)
                .Matches(EF.Functions.PlainToTsQuery(searchQuery)))
            .ProjectTo<VideoMinGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}