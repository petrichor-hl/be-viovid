using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Channel;

public class GetChannelRequest
{
    [Required] public Guid Id { get; set; }
}