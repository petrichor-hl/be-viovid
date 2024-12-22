using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payment;

public class StripeCallbackRequest
{
    [Required]
    public string sessionId { get; set; } = null!;
}