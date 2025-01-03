using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Post;

public class GetPostRequest
{
    [Required] public Guid Id { get; set; }
}