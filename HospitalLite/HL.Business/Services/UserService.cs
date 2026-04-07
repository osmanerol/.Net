using HL.Core.DTOs;
using HL.Core.Entities;
using HL.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<AppUser> _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IGenericRepository<AppUser> userRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateUserDto dto)
        {
            AppUser user = new()
            {
                CreatedBy = "admin",
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };
            await _userRepo.AddAsync(user);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateRefreshToken(int userId, string refreshToken, DateTime expiration)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if(user != null)
            {
                user.RefreshToken = refreshToken;
                user.ReftreshTokenEndDate = expiration;
                await _unitOfWork.CommitAsync();
            }
        }
    }  
}
