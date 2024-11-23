using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class Review
{
    public Guid Id { get; set; }
    public int Start { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreateAt { get; set; }
    
    public Guid FilmId { get; set; }
    public Guid ApplicationUserId { get; set; }

    public Film Film { get; set; } = null!;
    public ApplicationUser ApplicationUser { get; set; } = null!;
}