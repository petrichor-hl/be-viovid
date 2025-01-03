using Application.DTOs.Film.Res;

namespace Application.DTOs.Topic.Res;

public class TopicResponse
{
    public Guid TopicId { get; set; }
    
    public string Name { get; set; } = null!;
    
    public int Order { get; set; }
    
    public List<SimpleFilmResponse> Films { get; set; } = new List<SimpleFilmResponse>(); 
}