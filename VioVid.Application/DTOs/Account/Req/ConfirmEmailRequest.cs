using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account.Req;

public class ConfirmEmailRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string VerifyEmailToken { get; set; } = string.Empty;
}