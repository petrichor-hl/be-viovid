using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Dashboard.Req;

public class GetUserRegistrationStatsRequest
{
    [Required]
    public int Year { get; set; }
}