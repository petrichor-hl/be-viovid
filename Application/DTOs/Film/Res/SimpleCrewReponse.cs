namespace Application.DTOs.Film.Res;

public class SimpleCrewReponse
{
    public Guid CrewId { get; set; }
    public string Role { get; set; } = null!;
    public string PersonName { get; set; } = null!;
    public string? PersonProfilePath { get; set; }
}