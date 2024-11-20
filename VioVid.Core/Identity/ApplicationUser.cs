using Microsoft.AspNetCore.Identity;

namespace VioVid.Core.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string NickName { get; set; } = string.Empty;
        public string Avatar {  get; set; } = string.Empty;
        public int TokenVersion { get; set; }   // Default equal to 0
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDateTime { get; set; }
    }
}

