using HL.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace HL.Core.Entities
{
    public class UserClaim : BaseEntity
    {
        public int AppUserId { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; }
    }
}
