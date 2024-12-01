using Application.DTOs;
using Application.DTOs.User.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("profile")]
    public async Task<IActionResult> Logout()
    {
        return Ok(ApiResult<UserProfileResponse>.Success(await _userService.GetUserProfileAsync()));
    }
}