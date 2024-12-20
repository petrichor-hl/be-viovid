namespace Application.DTOs.User.Res;

public class UserPlanResponse
{
    public Guid UserPlanId { get; set; }
    
    public string PlanName { get; set; }
    public int Amount { get; set; }
    
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}