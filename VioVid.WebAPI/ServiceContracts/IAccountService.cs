using Application.DTOs.Account;

namespace VioVid.WebAPI.ServiceContracts;

public interface IAccountService
{
    public Task<Guid> Register(RegisterRequest registerRequest);
    
    public Task<LoginResponse> Login(LoginRequest loginRequest);

    public Task<bool> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest);
}