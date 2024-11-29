using Application.DTOs;
using Application.DTOs.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Identity;
using VioVid.Core.ServiceContracts;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest)
    {
        return Ok(ApiResult<Guid>.Success(await _accountService.Register(registerRequest)));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public  async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        return Ok(ApiResult<LoginResponse>.Success(await _accountService.Login(loginRequest)));
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        return Ok(ApiResult<bool>.Success(await _accountService.ConfirmEmail(confirmEmailRequest)));
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> GenerateNewAccessToken(RefreshTokenDto refreshTokenDto)
    {
        return Ok(ApiResult<RefreshTokenDto>.Success(await _accountService.RefreshToken(refreshTokenDto)));
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        return Ok(ApiResult<bool>.Success(await _accountService.Logout(User)));
    }
}
