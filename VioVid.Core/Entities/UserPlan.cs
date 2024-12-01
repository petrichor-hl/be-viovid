using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class UserPlan
{
    public Guid Id { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    
    public Guid ApplicationUserId { get; set; }
    public Guid PlanId { get; set; }

    public ApplicationUser ApplicationUser { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}