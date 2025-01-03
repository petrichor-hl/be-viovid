using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PostComment;

public class CreatePostCommentRequest
{

    [Required] public Guid PostId { get; set; }

    [Required] public string Content { get; set; }
}