using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Post;

public class CreatePostRequest
{

    [Required] public Guid ChannelId { get; set; }

    [Required] public string[] Hashtags { get; set; }

    [Required] public string Content { get; set; }

    public string[] ImageUrls { get; set; }
}