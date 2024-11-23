namespace VioVid.Core.Entities;

public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<GenreFilm> GenreFilms { get; set; } = null!;
}