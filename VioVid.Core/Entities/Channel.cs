namespace VioVid.Core.Entities;

public class Channel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null;
    public DateTime CreatedAt { get; set; }
}