using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Channel;

public class CreateChannelRequest
{
    [Required] public string Name { get; set; }
}