using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Film.Req;

public class PostReviewRequest
{
    [Required]
    [Range(1, 5, ErrorMessage = "Start phải nằm trong khoảng từ {1} đến {2}.")]
    public int Start { get; set; }
    
    [Required]
    public string Content { get; set; } = string.Empty;
}