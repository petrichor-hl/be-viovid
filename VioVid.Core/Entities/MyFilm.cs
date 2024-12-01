namespace VioVid.Core.Entities;

public class MyFilm
{
    public Guid Id { get; set; }
    public Guid ApplicationUserId { get; set; }
    public Guid FilmId { get; set; }
    
    public Film Film { get; set; } = null!;
}