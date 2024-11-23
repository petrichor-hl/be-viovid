using Microsoft.AspNetCore.Identity;
using VioVid.Core.Entities;

namespace VioVid.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int TokenVersion { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDateTime { get; set; }

        public UserProfile UserProfile { get; set; } = null!;
    }
}

