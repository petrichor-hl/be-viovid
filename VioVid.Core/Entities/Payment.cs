using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDone { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string PlanName { get; set; } = null!;
    public int? Amount { get; set; }
    
    public string MethodName { get; set; } = null!;
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
    
    public Guid? PlanId { get; set; }
    public Plan? Plan { get; set; } = null!;
}