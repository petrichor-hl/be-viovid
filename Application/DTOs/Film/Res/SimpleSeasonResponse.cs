namespace Application.DTOs.Film.Res;

public class SimpleSeasonResponse
{
    public Guid Id { get; set; }
    public int Order { get; set; }
    public string Name { get; set; } = null!;
}