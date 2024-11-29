using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Plan;

public class CreatePlanRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public int Price  { get; set; }
    
    [Required]
    public int Duration { get; set; }
}