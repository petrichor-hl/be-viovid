using Application.DTOs.Film.Res;

namespace Application.DTOs.Topic.Res;

public class BrowseTopicResponse : TopicResponse
{
    public List<SimpleFilmResponse> Films { get; set; } = new List<SimpleFilmResponse>(); 
}