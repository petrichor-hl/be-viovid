using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class ChangePasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}