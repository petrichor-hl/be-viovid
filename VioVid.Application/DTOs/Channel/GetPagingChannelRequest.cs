using Application.Models;

namespace Application.DTOs.Channel;

public class GetPagingChannelRequest : PaginationFilter
{
    public string? SearchText { get; set; } = null;
}