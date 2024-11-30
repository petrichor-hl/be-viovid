namespace VioVid.Core.Entities;

public class Cast
{
    public Guid Id { get; set; }
    public string Character { get; set; } = null!;
    
    public Guid FilmId { get; set; }
    public Film Film { get; set; } = null!;

    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
}