using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PostComment;

public class GetPostCommentRequest
{
    [Required] public Guid Id { get; set; }
}