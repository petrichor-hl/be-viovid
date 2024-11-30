using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Topic.Req;

public class UpdateTopicRequest : CreateTopicRequest
{
    public int? Order { get; set; }
}