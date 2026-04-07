using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.DTOs
{
    public record TokenDto(string AccessToken, DateTime AccessTokenExpiration, string RefreshToken);
}
