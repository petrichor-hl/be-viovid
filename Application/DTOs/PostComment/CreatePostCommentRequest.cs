using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PostComment;

public class CreatePostCommentRequest
{
    [Required] public Guid ApplicationUserId { get; set; }

    [Required] public Guid PostId { get; set; }

    [Required] public string Content { get; set; }
}