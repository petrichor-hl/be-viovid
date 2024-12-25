namespace VioVid.Core.Entities;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int Price  { get; set; }
    public int Duration { get; set; }
    // public int Order { get; set; }
    
    public ICollection<Payment> Payments { get; set; } = null!;
}