using AutoMapper;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Domain.Exceptions;

namespace OurTube.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserService(IApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<ApplicationUserDto> UpdateUserAsync(ApplicationUserPatchDto patchDto, Guid userId)
    {
        var aUser = await _dbContext.ApplicationUsers
            .GetByIdAsync(userId,true);

        var iUser = await _dbContext.IdentityUsers.FindAsync(userId);

        if (iUser == null)
            throw new NotFoundException(typeof(IdentityUser), userId);

        if (patchDto.UserName != null)
        {
            aUser.UserName = patchDto.UserName;
            iUser.UserName = patchDto.UserName;
        }

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<ApplicationUserDto>(aUser);
    }

    public async Task<ApplicationUserDto> GetUserAsync(Guid userId)
    {
        return _mapper.Map<ApplicationUserDto>(await _dbContext.ApplicationUsers.GetByIdAsync(userId));
    }
}