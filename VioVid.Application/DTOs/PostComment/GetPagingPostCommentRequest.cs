using System.ComponentModel.DataAnnotations;
using Application.Models;

namespace Application.DTOs.PostComment;

public class GetPagingPostCommentRequest : PaginationFilter
{
    [Required] public Guid PostId { get; set; }
}