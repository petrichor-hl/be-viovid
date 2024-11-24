using Application.DTOs;

namespace Application.Exceptions;

public class ApplicationException : Exception
{
    public ApiResultErrorCodes Code { get; }
    protected ApplicationException(ApiResultErrorCodes code, string message) : base(message)
    {
        Code = code;
    }
}