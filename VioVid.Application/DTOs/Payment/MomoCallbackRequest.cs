using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payment;

public class MomoCallbackRequest
{
    [Required]
    public Guid PaymentId { get; set; }
    
    [Required]
    public int ResultCode { get; set; }
    
    [Required]
    public int Amount { get; set; }
}