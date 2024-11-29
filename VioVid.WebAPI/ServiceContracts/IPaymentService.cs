using Application.DTOs.Genre;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPaymentService
{
    Task<List<Genre>> GetAllAsync();

    Task<Genre> GetByIdAsync(Guid id);

    Task<Genre> CreateGenreAsync(CreateGenreRequest createGenreRequest);

    Task<Genre> UpdateGenreAsync(Guid id, UpdateGenreRequest updateGenreRequest);

    Task<Guid> DeleteGenreAsync(Guid id);
}