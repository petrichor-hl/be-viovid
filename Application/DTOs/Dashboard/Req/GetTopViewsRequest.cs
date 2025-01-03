using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Dashboard.Req;

public class GetTopViewsRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Count must be greater than 0.")]
    public int Count { get; set; }
}