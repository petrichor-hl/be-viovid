using Application.DTOs.Film.Res;

namespace Application.DTOs.Genre.Res;

public class GenreResponse : SimpleGenreResponse
{
    public List<SimpleFilmResponse> Films { get; set; } = new List<SimpleFilmResponse>();
}