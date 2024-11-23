namespace VioVid.Core.Entities;

public class Film
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Overview { get; set; } = null!;
    public string BackdropPath { get; set; } = null!;
    public string PosterPath { get; set; } = null!;
    public string ContentRating { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    
    public ICollection<Season> Seasons { get; set; } = null!;
}