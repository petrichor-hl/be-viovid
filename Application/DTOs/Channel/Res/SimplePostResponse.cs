namespace Application.DTOs.Channel.Res;

public class SimplePostResponse
{
    public Guid Id { get; set; }
    public string OwnerName { get; set; } = null!;
    public string OwnerAvatar { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string[] Hashtags { get; set; } = null!;
    public string Content { get; set; }
    public string[] ImageUrls { get; set; } = null!;
    public int Likes { get; set; }
}