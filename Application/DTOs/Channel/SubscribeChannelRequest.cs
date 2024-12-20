using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Channel;

public class SubscribeChannelRequest
{
    [Required] public Guid ChannelId { get; set; }
}