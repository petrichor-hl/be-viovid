using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Req;

public class UpdateTrackingProgressRequest
{
    [Required]
    public Guid EpisodeId { get; set; }
    
    [Required]
    public int Progress { get; set; }
}