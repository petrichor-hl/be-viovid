using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class UpdateFcmTokenRequest
{
    [Required]
    public string FcmToken { get; set; } = string.Empty;
}