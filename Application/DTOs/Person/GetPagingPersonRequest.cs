using Application.Models;

namespace Application.DTOs.Person;

public class GetPagingPersonRequest : PaginationFilter
{
    public string? SearchText { get; set; }
}