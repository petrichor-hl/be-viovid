using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Genre;

public class UpdateGenreRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
}