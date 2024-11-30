using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Topic.Req;

public class CreateTopicRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}