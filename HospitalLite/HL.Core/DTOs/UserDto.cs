using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.DTOs
{
    public record UserDto(int Id, string FirstName, string LastName, string Email);
}
