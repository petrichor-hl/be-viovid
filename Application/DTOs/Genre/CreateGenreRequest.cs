using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Genre;

public class CreateGenreRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}