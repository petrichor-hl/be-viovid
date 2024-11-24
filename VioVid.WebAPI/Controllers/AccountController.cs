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
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        return Ok(ApiResult<Guid>.Success(await _accountService.Register(registerDto)));
    }
}

