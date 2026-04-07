using HL.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.Interface
{
    public interface IAuthService
    {
        Task<TokenDto> LoginAsync(LoginDto dto);
    }
}
