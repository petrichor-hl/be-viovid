using System.ComponentModel.DataAnnotations;
using VioVid.Core.Enum;

namespace Application.DTOs.Person;

public class CreatePersonRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    public double Popularity { get; set; }
    
    [Required]
    public string? ProfilePath { get; set; } = string.Empty;
    
    [Required]
    public string? Biography { get; set; }
    
    [Required]
    public string? KnownForDepartment { get; set; }
    
    [Required]
    public DateOnly? Dob { get; set; }
}