using Microsoft.AspNetCore.Identity;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public UserService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task UpdateUser(ApplicationUserPatchDto patchDto, string userId)
        {
            var aUser = _unitOfWorks.ApplicationUsers.Get(userId);

            if (aUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            var iUser = _unitOfWorks.IdentityUsers.Get(userId);

            if (iUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            if (patchDto.UserName != null)
            {
                aUser.UserName = patchDto.UserName;
                iUser.UserName = patchDto.UserName;
            }

            await _unitOfWorks.SaveChangesAsync();
        }
    }
}
