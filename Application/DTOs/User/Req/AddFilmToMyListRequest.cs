using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Req;

public class AddFilmToMyListRequest
{
    [Required]
    public Guid FilmId { get; set; }
}