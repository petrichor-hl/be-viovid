using Application.Models;

namespace Application.DTOs.Film.Req;

public class GetPagingFilmRequest : PaginationFilter
{
    public string? SearchText { get; set; }
}
