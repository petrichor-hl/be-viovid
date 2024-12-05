namespace Application.DTOs.Topic.Req;

public class AddFilmsToTopicRequest
{
    public IEnumerable<Guid> FilmIds { get; set; } = new List<Guid>();
}