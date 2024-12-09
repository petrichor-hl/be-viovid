using Application.DTOs;
using Application.DTOs.Film.Res;
using Application.DTOs.User.Req;
using Application.DTOs.User.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostCommentController : ControllerBase
{
    private readonly IUserService _userService;

    public PostCommentController(IUserService userService)
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
        return Ok(
            ApiResult<SimpleFilmResponse>.Success(await _userService.AddFilmToMyListAsync(addFilmToMyListRequest)));
    }

    [HttpDelete("my-list/{filmId:guid}")]
    public async Task<IActionResult> AddFilmToMyList(Guid filmId)
    {
        return Ok(ApiResult<Guid>.Success(await _userService.RemoveFilmFromMyListByFilmIdAsync(filmId)));
    }
}