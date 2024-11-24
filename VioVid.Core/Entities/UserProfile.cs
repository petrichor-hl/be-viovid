using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class UserProfile
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Avatar {  get; set; } = null!;
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public ICollection<UserPlan> UserPlans { get; set; } = null!;
}