using Application.DTOs.Account;

namespace VioVid.WebAPI.ServiceContracts;

public interface IAccountService
{
    public Task<Guid> Register(RegisterDto registerDto);
}