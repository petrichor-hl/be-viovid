namespace Application.DTOs.Film.Res;

public class SimpleFilmResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
}