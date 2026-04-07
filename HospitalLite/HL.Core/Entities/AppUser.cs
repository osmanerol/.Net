using HL.Core.Common;
using HL.Core.Enums;

namespace HL.Core.Entities
{
    public class AppUser : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ReftreshTokenEndDate { get; set; }
        public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    }
}
