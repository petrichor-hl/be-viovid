using System.Security.Claims;
using Application.DTOs.Account;

namespace VioVid.WebAPI.ServiceContracts;

public interface IAccountService
{
    Task<Guid> Register(RegisterRequest registerRequest);
    
    Task<LoginResponse> Login(LoginRequest loginRequest);

    Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
    
    Task<RefreshTokenDto> RefreshToken(RefreshTokenDto refreshTokenDto);
    
    Task<bool> Logout();

    Task<Guid> DeleteAccount();
}