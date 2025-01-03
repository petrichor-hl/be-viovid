using Application.DTOs;

namespace Application.Exceptions;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message): base(ApiResultErrorCodes.NotFound, message)
    {
    }
}