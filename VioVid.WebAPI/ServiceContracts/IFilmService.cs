using Application.DTOs.Film.Req;
using Application.DTOs.Film.Res;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IFilmService
{
    Task<PaginationResponse<SimpleFilmResponse>> GetAllAsync(GetPagingFilmRequest getPagingFilmRequest);
    
    Task<FilmResponse> GetByIdAsync(Guid id);
    
    // Task<List<Season>> GetSeasonsAsync(Guid id);
    
    Task<SeasonResponse> GetSeasonsAsync(Guid filmId, Guid seasonId);
    
    Task<List<Review>> GetReviewsAsync(Guid id);
    
    Task<List<SimpleCastResponse>> GetCastsAsync(Guid id);
    
    Task<List<SimpleCrewReponse>> GetCrewsAsync(Guid id);
    
    Task<Film> CreateFilmAsync(CreateFilmRequest createFilmRequest);

    Task<Film> UpdateFilmAsync(Guid id, UpdateFilmRequest updateFilmRequest);
    
    Task<Guid> DeleteFilmAsync(Guid id);
}