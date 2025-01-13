using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.User.Req;

public class UpdateThreadIdRequest
{
    public string? ThreadId { get; set; }
}