namespace Application.DTOs.Film.Res;

public class SimpleFilmResponse
{
    public Guid FilmId { get; set; }
    public string Name { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
}