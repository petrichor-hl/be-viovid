using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class PostComment
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;
}
