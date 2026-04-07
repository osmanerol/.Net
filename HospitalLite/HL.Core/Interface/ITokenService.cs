using HL.Core.DTOs;
using HL.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.Interface
{
    public interface ITokenService
    {
        TokenDto CreateToken(AppUser user);
    }
}
