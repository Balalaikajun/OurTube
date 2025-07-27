using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Application.Requests.ApplicationUser;
using OurTube.Domain.Entities;
using OurTube.Domain.Exceptions;
using ApplicationUser = OurTube.Application.Replies.ApplicationUser.ApplicationUser;

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

    public async Task<ApplicationUser> UpdateUserAsync(PatchApplicationUserRequest patchDto, Guid userId)
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

        return _mapper.Map<ApplicationUser>(aUser);
    }

    public async Task<ApplicationUser> GetUserAsync(Guid userId)
    {
        return await _dbContext.ApplicationUsers.GetByIdAsync<Domain.Entities.ApplicationUser,ApplicationUser>(userId, _mapper.ConfigurationProvider);
    }
}