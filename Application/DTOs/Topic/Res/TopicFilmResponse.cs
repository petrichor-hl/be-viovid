using Application.DTOs.Film.Res;

namespace Application.DTOs.Topic.Res;

public class TopicFilmResponse
{
    public Guid TopicId { get; set; }
    public string Name { get; set; } = null!;
    
    public List<SimpleFilmResponse> Films { get; set; } = new List<SimpleFilmResponse>(); 
}