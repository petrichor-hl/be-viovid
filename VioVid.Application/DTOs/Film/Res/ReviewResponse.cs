namespace Application.DTOs.Film.Res;

public class ReviewResponse
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = null!;
    public string UserAvatar { get; set; } = null!;
    public int Start { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreateAt { get; set; }
}