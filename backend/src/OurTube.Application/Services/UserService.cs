using Microsoft.AspNetCore.Identity;
using OurTube.Application.DTOs.ApplicationUser;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task UpdateUserAsync(ApplicationUserPatchDto patchDto, string userId)
        {
            var aUser =await _unitOfWork.ApplicationUsers.GetAsync(userId);

            if (aUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            var iUser = await _unitOfWork.IdentityUsers.GetAsync(userId);

            if (iUser == null)
                throw new KeyNotFoundException("Пользователь не найден");

            if (patchDto.UserName != null)
            {
                aUser.UserName = patchDto.UserName;
                iUser.UserName = patchDto.UserName;
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
