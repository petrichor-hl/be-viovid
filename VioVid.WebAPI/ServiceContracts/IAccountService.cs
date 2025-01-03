using Application.DTOs.Account;
using Application.DTOs.Account.Req;
using Application.DTOs.Account.Res;

namespace VioVid.WebAPI.ServiceContracts;

public interface IAccountService
{
    Task<Guid> Register(RegisterRequest registerRequest);
    
    Task<LoginResponse> Login(LoginRequest loginRequest);

    Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
    
    Task<RefreshTokenDto> RefreshToken(RefreshTokenDto refreshTokenDto);

    Task<bool> UpdateFcmToken(UpdateFcmTokenRequest updateFcmTokenRequest);
    
    Task<bool> Logout();

    Task<Guid> DeleteAccount(string userId);
    
    Task<bool> ChangePassword(ChangePasswordRequest changePasswordRequest);
    
    Task<List<AccountResponse>> GetAllAccounts(GetAccountRequest getAccountRequest);
}