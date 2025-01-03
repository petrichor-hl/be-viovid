using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Application.DTOs.Post;

public class GetPagingPostRequest : PaginationFilter
{
    [Required] public Guid ChannelId { get; set; }
}