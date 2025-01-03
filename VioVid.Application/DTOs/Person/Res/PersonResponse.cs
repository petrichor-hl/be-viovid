using Application.DTOs.Film.Res;
using VioVid.Core.Enum;

namespace Application.DTOs.Person.Res;

public class PersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Gender Gender { get; set; }
    public double Popularity { get; set; }
    public string? ProfilePath { get; set; } = null!;
    public string? Biography { get; set; }
    public string? KnownForDepartment { get; set; }
    public DateOnly? Dob { get; set; }
    
    public List<SimpleFilmResponse> Films { get; set; } = null!;
}