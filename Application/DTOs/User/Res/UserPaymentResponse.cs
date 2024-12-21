namespace Application.DTOs.User.Res;

public class UserPaymentResponse
{
    public Guid PaymentId { get; set; }
    public string PlanName { get; set; } = null!;
    public bool IsDone { get; set; }
    public int? Amount { get; set; }
    
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}