namespace Application.DTOs.Account.Res;

public class LoginResponse
{
    public string Email { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;
}