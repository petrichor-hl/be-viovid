using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class UpdateFcmTokenDto
{
    [Required]
    public string FcmToken { get; set; } = string.Empty;
}