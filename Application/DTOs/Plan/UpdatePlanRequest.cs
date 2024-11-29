using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Plan;

public class UpdatePlanRequest
{
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    public int Price  { get; set; }
    
    [Required]
    public int Duration { get; set; }
    
    [Required]
    public int Order { get; set; }
}