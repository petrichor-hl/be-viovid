namespace VioVid.Core.Entities;

public class Season
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Name { get; set; } = null!;
    
    public Guid FilmId { get; set; }
    public Film Film { get; set; } = null!;
    
    public ICollection<Episode> Episodes { get; set; } = null!;
}