using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Req;

public class AddUserPaymentRequest
{
    [Required]
    public Guid PlanId { get; set; }
}