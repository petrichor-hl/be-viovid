using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.Topic.Req;

public class ReorderTopicsRequest
{
    [Required]
    public int OldIndex { get; set; }
    
    [Required]
    public int NewIndex { get; set; }
}