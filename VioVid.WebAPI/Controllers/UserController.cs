using Application.DTOs;
using Application.DTOs.Account;
using Application.DTOs.Film.Res;
using Application.DTOs.User.Req;
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
    public async Task<IActionResult> GetProfile()
    {
        return Ok(ApiResult<UserProfileResponse>.Success(await _userService.GetUserProfileAsync()));
    }
    
    [HttpGet("my-list")]
    public async Task<IActionResult> GetMyList()
    {
        return Ok(ApiResult<List<SimpleFilmResponse>>.Success(await _userService.GetMyListAsync()));
    }
    
    [HttpPost("my-list")]
    public async Task<IActionResult> AddFilmToMyList(AddFilmToMyListRequest addFilmToMyListRequest)
    {
        return Ok(ApiResult<SimpleFilmResponse>.Success(await _userService.AddFilmToMyListAsync(addFilmToMyListRequest)));
    }
    
    [HttpDelete("my-list/{filmId:guid}")]
    public async Task<IActionResult> AddFilmToMyList(Guid filmId)
    {
        return Ok(ApiResult<Guid>.Success(await _userService.RemoveFilmFromMyListByFilmIdAsync(filmId)));
    }
    
    [HttpGet("tracking-progress")]
    public async Task<IActionResult> GetTrackingProgress()
    {
        return Ok(ApiResult<List<TrackingProgressResponse>>.Success(await _userService.GetTrackingProgressAsync()));
    }
    
    [HttpPost("tracking-progress")]
    public async Task<IActionResult> UpdateTrackingProgress(UpdateTrackingProgressRequest updateTrackingProgressRequest)
    {
        return Ok(ApiResult<bool>.Success(await _userService.UpdateTrackingProgressAsync(updateTrackingProgressRequest)));
    }
    
    // Just ignore this endpoint
    [HttpPost("user-payments")]
    public async Task<IActionResult> AddUserPayment(AddUserPaymentRequest addUserPaymentRequest)
    {
        return Ok(ApiResult<bool>.Success(await _userService.AddUserPayment(addUserPaymentRequest)));
    }
}