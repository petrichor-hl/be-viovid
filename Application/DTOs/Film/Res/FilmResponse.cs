namespace Application.DTOs.Film.Res;

public class FilmResponse : SimpleFilmResponse
{
    public string Overview { get; set; } = null!;
    public string BackdropPath { get; set; } = null!;
    public string ContentRating { get; set; } = null!;
    public DateOnly? ReleaseDate { get; set; }
    
    public string TmdbId { get; set; } = null!;
    
    public List<SeasonResponse> Seasons { get; set; } = null!;
    public List<SimpleGenreResponse> Genres { get; set; } = null!;
}