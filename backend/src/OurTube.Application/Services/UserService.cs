using AutoMapper;
using Microsoft.AspNetCore.Identity;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OurTube.Application.Services
{
    public class UserService
    {
        private ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateUser(ApplicationUserPatchDTO patchDTO, string userId)
        {
            ApplicationUser aUser = _dbContext.ApplicationUsers.First(au => au.Id==userId);

            IdentityUser iUser = _dbContext.Users.First(iu => iu.Id == userId);

            if (patchDTO.UserName != null)
            {
                aUser.UserName = patchDTO.UserName;
                iUser.UserName = patchDTO.UserName;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
