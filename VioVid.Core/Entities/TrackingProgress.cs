using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class TrackingProgress
{
    public Guid Id { get; set; }
    public int Progress { get; set; }
    
    public Guid EpisodeId { get; set; }
    public Episode Episode { get; set; }
    
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
}