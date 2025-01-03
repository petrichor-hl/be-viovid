using Application.DTOs.Film.Req;
using Application.DTOs.Film.Res;
using VioVid.Core.Common;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IFilmService
{
    Task<PaginationResponse<SimpleFilmResponse>> GetAllAsync(GetPagingFilmRequest getPagingFilmRequest);
    
    Task<FilmResponse> GetByIdAsync(Guid id);
    
    Task<List<ReviewResponse>> GetReviewsAsync(Guid id);
    
    Task<ReviewResponse> PostReview(Guid filmId, PostReviewRequest postReviewRequest);
    
    Task<List<SimpleCastResponse>> GetCastsAsync(Guid id);
    
    Task<List<SimpleCrewReponse>> GetCrewsAsync(Guid id);
    
    Task<Film> CreateFilmAsync(CreateFilmRequest createFilmRequest);

    Task<Film> UpdateFilmAsync(Guid id, UpdateFilmRequest updateFilmRequest);
    
    Task<Guid> DeleteFilmAsync(Guid id);
    
    Task<bool> CountViewForFilmAsync(Guid id);
}