namespace VioVid.Core.Entities;

public class PostComment
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = null!;

    public ICollection<Person> User { get; set; } = null!;
    public ICollection<Post> Post { get; set; } = null!;
}