namespace VioVid.Core.Entities;

public class Crew
{
    public Guid Id { get; set; }
    public string Role { get; set; } = null!;
    
    public Guid FilmId { get; set; }
    public Guid PersonId { get; set; }

    public Film Film { get; set; } = null!;
    public Person Person { get; set; } = null!;
}