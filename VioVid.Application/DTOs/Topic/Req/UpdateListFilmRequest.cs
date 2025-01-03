using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Topic.Req;

public class UpdateListFilmRequest
{
    [Required]
    public List<Guid> FilmIds { get; set; } = null!;
}