namespace Application.DTOs.Account.Res;
public class AccountResponse
{
    public Guid ApplicationUserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; } = null!;
    public string Avatar {  get; set; } = null!;
    
    public bool EmailConfirmed { get; set; }
}