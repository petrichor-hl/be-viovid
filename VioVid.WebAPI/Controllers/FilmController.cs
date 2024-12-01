using Application.DTOs;
using Application.DTOs.Film;
using Application.DTOs.Film.Req;
using Application.DTOs.Film.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Common;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FilmController : ControllerBase
{
    private readonly IFilmService _filmService;

    public FilmController(IFilmService filmService)
    {
        _filmService = filmService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] GetPagingFilmRequest getPagingFilmRequest)
    {
        return Ok(ApiResult<PaginationResponse<SimpleFilmResponse>>.Success(await _filmService.GetAllAsync(getPagingFilmRequest)));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFilmById(Guid id)
    {
        return Ok(ApiResult<FilmResponse>.Success(await _filmService.GetByIdAsync(id)));
    }

    
    [HttpGet("{filmId:guid}/seasons/{seasonId:guid}")]
    public async Task<IActionResult> GetSeasons(Guid filmId, Guid seasonId)
    {
        return Ok(ApiResult<SeasonResponse>.Success(await _filmService.GetSeasonsAsync(filmId, seasonId)));
    }
    
    [HttpGet("{filmId:guid}/reviews")]
    public async Task<IActionResult> GetReviews(Guid filmId)
    {
        return Ok(ApiResult<List<ReviewResponse>>.Success(await _filmService.GetReviewsAsync(filmId)));
    }
    
    [HttpPost("{filmId:guid}/reviews")]
    public async Task<IActionResult> PostReview(Guid filmId, PostReviewRequest postReviewRequest)
    {
        return Ok(ApiResult<ReviewResponse>.Success(await _filmService.PostReview(filmId, postReviewRequest)));
    }
    
    [HttpGet("{id:guid}/casts")]
    public async Task<IActionResult> GetCasts(Guid id)
    {
        return Ok(ApiResult<List<SimpleCastResponse>>.Success(await _filmService.GetCastsAsync(id)));
    }
    
    [HttpGet("{id:guid}/crews")]
    public async Task<IActionResult> GetCrews(Guid id)
    {
        return Ok(ApiResult<List<SimpleCrewReponse>>.Success(await _filmService.GetCrewsAsync(id)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateFilm(CreateFilmRequest createFilmRequest)
    {
        return Ok(ApiResult<Film>.Success(await _filmService.CreateFilmAsync(createFilmRequest)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateFilm(Guid id,[FromBody] UpdateFilmRequest updateFilmRequest)
    {
        return Ok(ApiResult<Film>.Success(await _filmService.UpdateFilmAsync(id, updateFilmRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFilm(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _filmService.DeleteFilmAsync(id)));
    }
}