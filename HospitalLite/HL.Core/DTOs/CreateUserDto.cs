using HL.Core.Enums;
namespace HL.Core.DTOs
{
    public record CreateUserDto(string FirstName, string LastName, string Email, string Password, UserRole Role);
}
