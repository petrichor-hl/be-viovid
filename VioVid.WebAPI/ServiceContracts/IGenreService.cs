using Application.DTOs.Genre;
using Application.DTOs.Genre.Res;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IGenreService
{
    Task<List<Genre>> GetAllAsync();
    
    Task<GenreResponse> GetByIdAsync(Guid id);
    
    Task<Genre> CreateGenreAsync(CreateGenreRequest createGenreRequest);

    Task<Genre> UpdateGenreAsync(Guid id, UpdateGenreRequest updateGenreRequest);
    
    Task<Guid> DeleteGenreAsync(Guid id);
}