using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Notification.req;

public class UpdateReadStatusReq
{
    [Required]
    public Guid NotificationId { get; set; }
}