using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;
using OurTube.Application.DTOs;
using OurTube.Application.DTOs.Video;

namespace OurTube.Application.Services;

public class SearchService
{
    private IUnitOfWork _unitOfWork;
    private IMapper _mapper;

    public SearchService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<VideoMinGetDto>> SearchVideos(string searchQuery)
    {
        return await _unitOfWork.Videos
            .GetAll()
            .Where(v =>  EF.Functions.ToTsVector(v.Title)
                .Matches(EF.Functions.PlainToTsQuery(searchQuery)))
            .ProjectTo<VideoMinGetDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}