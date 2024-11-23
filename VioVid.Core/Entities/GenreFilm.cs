namespace VioVid.Core.Entities;

public class GenreFilm
{
    public Guid Id { get; set; }
    
    public Guid GenreId { get; set; }
    public Guid FilmId { get; set; }

    public Genre Genre { get; set; } = null!;
    public Film Film { get; set; } = null!;
}