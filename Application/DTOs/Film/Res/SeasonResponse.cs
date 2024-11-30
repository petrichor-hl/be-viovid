namespace Application.DTOs.Film.Res;

public class SeasonResponse : SimpleSeasonResponse
{
    public List<EpisodeResponse> Episodes { get; set; } = null!;
}