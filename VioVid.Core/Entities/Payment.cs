namespace VioVid.Core.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsDone { get; set; }
}