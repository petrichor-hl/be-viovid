using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Application.DTOs.Channel;

public class GetPagingChannelRequest : PaginationFilter
{
    [Required] public string? SearchText { get; set; }
}