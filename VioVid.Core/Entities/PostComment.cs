using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class PostComment
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Content { get; set; } = null!;
    
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}
