namespace VioVid.Core.Entities;

public class Film
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Overview { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public string BackdropPath { get; set; } = null!;
    public string ContentRating { get; set; } = null!;
    public DateOnly? ReleaseDate { get; set; }
    public string TmdbId { get; set; } = null!;
    public int NumberOfViews { get; set; }
    public ICollection<Season> Seasons { get; set; } = null!;
    public ICollection<GenreFilm> GenreFilms { get; set; } = null!;
    public ICollection<TopicFilm> TopicFilms { get; set; } = null!;
    public ICollection<Cast> Casts { get; set; } = null!;
    public ICollection<Crew> Crews { get; set; } = null!;
    public ICollection<Review> Reviews { get; set; } = null!;
}