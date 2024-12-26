using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.Post;

public class CreatePostRequest
{
    [Required] public Guid ChannelId { get; set; }
    [Required] public string[] Hashtags { get; set; }
    [Required] public string Content { get; set; }
    public ICollection<IFormFile>? Images { get; set; }
}