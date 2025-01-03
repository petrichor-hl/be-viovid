using Application.Models;

namespace Application.DTOs.Person.Req;

public class GetPagingPersonRequest : PaginationFilter
{
    public string? SearchText { get; set; }
}