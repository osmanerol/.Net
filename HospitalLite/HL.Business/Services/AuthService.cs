using HL.Core.DTOs;
using HL.Core.Entities;
using HL.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace HL.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<AppUser> _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public AuthService(IGenericRepository<AppUser> userRepo, IUnitOfWork unitOfWork, ITokenService tokenService, IUserService userService)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepo.Where(x => x.Email == loginDto.Email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                throw new Exception("Invalid email or password.");
            var tokenDto = _tokenService.CreateToken(user);
            await _userService.UpdateRefreshToken(user.Id, tokenDto.RefreshToken, DateTime.Now.AddDays(7));
            return tokenDto;
        }
    }
}
