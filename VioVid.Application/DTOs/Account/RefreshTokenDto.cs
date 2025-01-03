using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Account;

public class RefreshTokenDto
{
    [Required]
    public string AccessToken { get; set; } = string.Empty;
    
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}