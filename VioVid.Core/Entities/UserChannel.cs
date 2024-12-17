using VioVid.Core.Identity;

namespace VioVid.Core.Entities;

public class UserChannel
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public Guid ChannelId { get; set; }
    public Channel Channel { get; set; } = null!;
}