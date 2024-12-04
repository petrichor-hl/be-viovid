using Application.DTOs;
using Application.DTOs.Genre;
using Application.DTOs.Genre.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;
    
    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(ApiResult<List<Genre>>.Success(await _genreService.GetAllAsync()));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGenreById(Guid id)
    {
        return Ok(ApiResult<GenreResponse>.Success(await _genreService.GetByIdAsync(id)));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateGenre(CreateGenreRequest createGenreRequest)
    {
        return Ok(ApiResult<Genre>.Success(await _genreService.CreateGenreAsync(createGenreRequest)));
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateGenre(Guid id,[FromBody] UpdateGenreRequest updateGenreRequest)
    {
        return Ok(ApiResult<Genre>.Success(await _genreService.UpdateGenreAsync(id, updateGenreRequest)));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteGenre(Guid id)
    {
        return Ok(ApiResult<Guid>.Success(await _genreService.DeleteGenreAsync(id)));
    }
}