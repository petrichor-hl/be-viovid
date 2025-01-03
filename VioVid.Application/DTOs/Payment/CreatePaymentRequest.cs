using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Payment;

public class CreatePaymentRequest
{
    [Required]
    public Guid PlanId { get; set; }
}