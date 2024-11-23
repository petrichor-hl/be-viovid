namespace VioVid.Core.Entities;

public class Topic
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Order { get; set; }
    
    public ICollection<TopicFilm> TopicFilms { get; set; } = null!;
}