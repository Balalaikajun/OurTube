using Microsoft.AspNetCore.Identity;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class UserService
    {
        private IUnitOfWorks _unitOfWorks;

        public UserService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task UpdateUser(ApplicationUserPatchDTO patchDTO, string userId)
        {
            ApplicationUser aUser = _unitOfWorks.ApplicationUsers.Get(userId);

            if (aUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            IdentityUser iUser = _unitOfWorks.IdentityUsers.Get(userId);

            if (iUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            if (patchDTO.UserName != null)
            {
                aUser.UserName = patchDTO.UserName;
                iUser.UserName = patchDTO.UserName;
            }

            await _unitOfWorks.SaveChangesAsync();
        }
    }
}
