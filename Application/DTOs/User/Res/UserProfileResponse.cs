namespace Application.DTOs.User.Res;

public class UserProfileResponse
{
    public Guid ApplicationUserId { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Avatar {  get; set; } = null!;
    
    public string PlanName { get; set; } = null!;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}