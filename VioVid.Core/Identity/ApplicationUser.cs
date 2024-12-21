using Microsoft.AspNetCore.Identity;
using VioVid.Core.Entities;

namespace VioVid.Core.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public int TokenVersion { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationDateTime { get; set; }
    public string? FcmToken { get; set; }
    public UserProfile UserProfile { get; set; } = null!;
    // public ICollection<UserPlan> UserPlans { get; set; } = null!;
    public ICollection<Payment> Payments { get; set; } = null!;
    public ICollection<MyFilm> MyFilms { get; set; } = null!;
    
    public ICollection<UserChannel> Channels { get; set; } = null!;
    public ICollection<Post> Posts { get; set; } = null!;
    public ICollection<PostComment> PostComments { get; set; } = null!;
}