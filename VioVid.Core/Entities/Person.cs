using VioVid.Core.Enum;

namespace VioVid.Core.Entities;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Gender Gender { get; set; }
    public int Popularity { get; set; }
    public string ProfilePath { get; set; } = null!;
    public string? Biography { get; set; }
    public string? KnownForDepartment { get; set; }
    public DateTime? Dob { get; set; }
    
    public ICollection<Cast> Casts { get; set; } = null!;
    public ICollection<Crew> Crews { get; set; } = null!;
}