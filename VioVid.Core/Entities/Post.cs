using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class Post
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string[] Hashtags { get; set; } = null!;
    public string Content { get; set; }
    public string[] ImageUrls { get; set; } = null!;
    public int Likes { get; set; }
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
    
    public Guid ChannelId { get; set; }
    public Channel Channel { get; set; } = null!;
    
    public ICollection<PostComment> PostComments { get; set; } = null!;
}
