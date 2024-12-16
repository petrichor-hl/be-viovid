namespace Application.DTOs.Film.Res;

public class EpisodeResponse
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Title { get; set; } = null!;
    public string Summary { get; set; } = null!;
    public string Source { get; set; } = null!;
    public int Duration { get; set; }
    public string StillPath { get; set; } = null!;
    public bool IsFree { get; set; }
    public int Progress { get; set; }
}