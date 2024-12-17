using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDone { get; set; }
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
    
    public Guid PlanId { get; set; }
    public Plan Plan { get; set; } = null!;
}