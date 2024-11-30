namespace Application.DTOs.Film.Res;

public class SimpleCastResponse
{
    public Guid CastId { get; set; }
    public string Character { get; set; } = null!;
    public string PersonName { get; set; } = null!;
    public string? PersonProfilePath { get; set; }
}