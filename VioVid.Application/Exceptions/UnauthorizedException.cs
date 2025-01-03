using Application.DTOs;

namespace Application.Exceptions;

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(ApiResultErrorCodes.Unauthorized, message) 
    {
    }
}