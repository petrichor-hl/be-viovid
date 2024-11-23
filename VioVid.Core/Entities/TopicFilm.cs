namespace VioVid.Core.Entities;

public class TopicFilm
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    
    public Guid TopicId { get; set; }
    public Guid FilmId { get; set; }
    
    public Topic Topic { get; set; } = null!;
    public Film Film { get; set; } = null!;
}