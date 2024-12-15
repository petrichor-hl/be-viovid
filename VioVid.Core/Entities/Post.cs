using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid ChannelId { get; set; }
    public string[] Hashtags { get; set; } = null!;
    public string Content { get; set; }
    public string[] ImageUrls { get; set; } = null!;
    public int Likes { get; set; }

    public ICollection<ApplicationUser> User { get; set; } = null!;
    public ICollection<Channel> Channel { get; set; } = null!;
}