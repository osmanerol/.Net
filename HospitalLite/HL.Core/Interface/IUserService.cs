using HL.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.Interface
{
    public interface IUserService
    {
        Task CreateAsync(CreateUserDto dto);
        Task UpdateRefreshToken(int userId, string refreshToken, DateTime expiration);
    }
}
